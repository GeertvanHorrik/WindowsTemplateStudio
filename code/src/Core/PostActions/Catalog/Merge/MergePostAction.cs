﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Templates.Core.Gen;

using Microsoft.Templates.Core.Resources;

namespace Microsoft.Templates.Core.PostActions.Catalog.Merge
{
    public class MergePostAction : PostAction<MergeConfiguration>
    {
        public MergePostAction(string relatedTemplate, MergeConfiguration config)
            : base(relatedTemplate, config)
        {
        }

        internal override void ExecuteInternal()
        {
            string originalFilePath = GetFilePath();
            if (!File.Exists(originalFilePath))
            {
                if (Config.FailOnError )
                {
                    throw new FileNotFoundException(string.Format(StringRes.MergeFileNotFoundExceptionMessage, Config.FilePath, RelatedTemplate));
                }
                else
                {
                    AddFailedMergePostActionsFileNotFound(originalFilePath);
                    File.Delete(Config.FilePath);
                    return;
                }
            }

            var source = File.ReadAllLines(originalFilePath).ToList();
            var merge = File.ReadAllLines(Config.FilePath).ToList();

            IEnumerable<string> result = source.Merge(merge, out string errorLine);

            if (errorLine != string.Empty)
            {
                if (Config.FailOnError)
                {
                    throw new InvalidDataException(string.Format(StringRes.MergeLineNotFoundExceptionMessage, errorLine, originalFilePath, RelatedTemplate));
                }
                else
                {
                    AddFailedMergePostActionsAddLineNotFound(originalFilePath, errorLine);
                }
            }
            else
            {
                Fs.EnsureFileEditable(originalFilePath);
                File.WriteAllLines(originalFilePath, result, Encoding.UTF8);

                // REFRESH PROJECT TO UN-DIRTY IT
                if (Path.GetExtension(Config.FilePath).EndsWith("proj", StringComparison.OrdinalIgnoreCase)
                 && GenContext.Current.OutputPath == GenContext.Current.DestinationPath)
                {
                    Gen.GenContext.ToolBox.Shell.RefreshProject();
                }
            }

            File.Delete(Config.FilePath);
        }

        protected void AddFailedMergePostActions(string originalFilePath, MergeFailureType mergeFailureType, string description)
        {
            var sourceFileName = GetRelativePath(originalFilePath);
            var postactionFileName = GetRelativePath(Config.FilePath);

            var failedFileName = GetFailedPostActionFileName();
            GenContext.Current.FailedMergePostActions.Add(new FailedMergePostActionInfo(sourceFileName, Config.FilePath, GetRelativePath(failedFileName), description, mergeFailureType));
            File.Copy(Config.FilePath, failedFileName, true);
        }

        protected string GetRelativePath(string path)
        {
            if (GenContext.Current.OutputPath == GenContext.Current.TempGenerationPath)
            {
                return path.Replace(GenContext.Current.OutputPath + Path.DirectorySeparatorChar, string.Empty);
            }
            else
            {
                return path.Replace(Directory.GetParent(GenContext.Current.OutputPath).FullName + Path.DirectorySeparatorChar, string.Empty);
            }
        }

        private void AddFailedMergePostActionsFileNotFound(string originalFilePath)
        {
            var description = string.Format(StringRes.FailedMergePostActionFileNotFound, GetRelativePath(originalFilePath), RelatedTemplate);
            AddFailedMergePostActions(originalFilePath,  MergeFailureType.FileNotFound, description);
        }

        private void AddFailedMergePostActionsAddLineNotFound(string originalFilePath, string errorLine)
        {
            var description = string.Format(StringRes.FailedMergePostActionLineNotFound, errorLine.Trim(), GetRelativePath(originalFilePath), RelatedTemplate);
            AddFailedMergePostActions(originalFilePath, MergeFailureType.LineNotFound, description);
        }

        private string GetFailedPostActionFileName()
        {
            var splittedFileName = Path.GetFileName(Config.FilePath).Split('.');
            splittedFileName[0] = splittedFileName[0].Replace(MergeConfiguration.Suffix, MergeConfiguration.NewSuffix);
            var folder = Path.GetDirectoryName(Config.FilePath);
            var extension = Path.GetExtension(Config.FilePath);

            var validator = new List<Validator>
            {
                new FileExistsValidator(Path.GetDirectoryName(Config.FilePath))
            };

            splittedFileName[0] = Naming.Infer(splittedFileName[0], validator);
            var newFileName = string.Join(".", splittedFileName);
            return Path.Combine(folder, newFileName);
        }

        private string GetFilePath()
        {
            if (Path.GetFileName(Config.FilePath).StartsWith(MergeConfiguration.Extension, StringComparison.OrdinalIgnoreCase))
            {
                var extension = Path.GetExtension(Config.FilePath);
                var directory = Path.GetDirectoryName(Config.FilePath);

                return Directory.EnumerateFiles(directory, $"*{extension}").FirstOrDefault(f => !f.Contains(MergeConfiguration.Suffix));
            }
            else
            {
                var path = Regex.Replace(Config.FilePath, MergeConfiguration.PostactionRegex, ".");

                return path;
            }
        }
    }
}
