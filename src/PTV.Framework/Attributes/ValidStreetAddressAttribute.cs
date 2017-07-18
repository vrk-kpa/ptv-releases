/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace PTV.Framework.Attributes
{
    /// <summary>
    /// Validates StreetAddress. If StreetAddress is defined either StreetNumber or coordinates (Latitude, Longitude) are required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidStreetAddressAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validate street address
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Value is not set, do not validate
            if (value == null)
            {
                return ValidationResult.Success;
            }

            // Collection must be an IList
            var addresses = value as IList;
            if (addresses == null)
            {
                throw new InvalidOperationException("The ValidStreetAddressAttribute is used on a type that doesn't implement IList.");
            }

            // Must contain at least one address
            if (addresses.Count > 0)
            {
                var address = validationContext.ObjectInstance;
                var streetNumber = address.GetType().GetProperty("StreetNumber")?.GetValue(address, null);

                // Has street number been given
                if (streetNumber != null)
                {
                    return ValidationResult.Success;
                }

                var latitudeProperty = address.GetType().GetProperty("Latitude");
                var longitudeProperty = address.GetType().GetProperty("Longitude");

                if (latitudeProperty == null)
                {
                    throw new InvalidOperationException("Required property 'Latitude' is not found.");
                }
                if (longitudeProperty == null)
                {
                    throw new InvalidOperationException("Required property 'Longitude' is not found.");
                }

                if (streetNumber == null && latitudeProperty != null && longitudeProperty != null)
                {
                    // Model has coordinate properties, validate accordingly
                    if (latitudeProperty != null && longitudeProperty != null)
                    {
                        var latitude = latitudeProperty.GetValue(address, null);
                        var longitude = longitudeProperty.GetValue(address, null);

                        if (latitude == null || longitude == null)
                        {
                            // StreetNumber and Coordinates are missing
                            return new ValidationResult(CoreMessages.OpenApi.ValidStreetAddressWithCoordinates);
                        }

                        return ValidationResult.Success;
                    }
                    // StreetNumber is missing (but model has no coordinates)
                    return new ValidationResult(CoreMessages.OpenApi.ValidStreetAddress);
                }
                else
                {
                    throw new InvalidOperationException("Required property 'StreetNumber' is not found.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
