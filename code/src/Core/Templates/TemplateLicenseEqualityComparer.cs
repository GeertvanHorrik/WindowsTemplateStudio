﻿// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using System.Collections.Generic;

namespace Microsoft.Templates.Core
{
    public class TemplateLicenseEqualityComparer : IEqualityComparer<TemplateLicense>
    {
        public bool Equals(TemplateLicense x, TemplateLicense y)
        {
            if (x == null && x == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }
            else if (x.Url == y.Url)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(TemplateLicense obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.Url))
            {
                return 0;
            }

            return obj.Url.GetHashCode();
        }
    }
}
