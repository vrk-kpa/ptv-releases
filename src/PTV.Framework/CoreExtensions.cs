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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Framework
{
    public static class Asyncs
    {
//        private static readonly TaskFactory MyTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

//        public static void RunSync(Func<Task> func)
//        {
//            var cultureUi = CultureInfo.CurrentUICulture;
//            var culture = CultureInfo.CurrentCulture;
//            MyTaskFactory.StartNew(() =>
//            {
//                Thread.CurrentThread.CurrentCulture = culture;
//                Thread.CurrentThread.CurrentUICulture = cultureUi;
//                return func();
//            }).Unwrap().GetAwaiter().GetResult();
//        }
        
        public static T HandleAsyncInSync<T>(Func<Task<T>> asyncFunc)
        {
            async Task<T> AsyncAction()
            {
                try
                {
                    return await asyncFunc();
                }
                catch (AggregateException e) when (e.InnerException is TaskCanceledException)
                {
                    throw new PtvActionCancelledException();
                }
                catch (AggregateException e) when (e.InnerException is PtvActionCancelledException)
                {
                    throw e.InnerException;
                }
                catch (TaskCanceledException)
                {
                    throw new PtvActionCancelledException();
                }
            }
            return AsyncAction().GetAwaiter().GetResult();
        }
    }

    public static class CoreExtensions
    {
        public static string ToBase64(this string rawString)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(rawString));
        } 
        
        public static string FromBase64(this string b64String)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(b64String));
        } 
        
        public static T DeserializeJsonObject<T>(this string jsonText, T defaultIfError = default(T))
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonText);
            }
            catch (Exception)
            {
                return defaultIfError;
            }
        }
        
        public static bool IsEven(this int value)
        {
            return value % 2 == 0;
        }

        public static char GetFirstChar(this string strChar)
        {
            return string.IsNullOrEmpty(strChar) ? default(char) : char.ToLower(strChar.First());
        }
        
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> enumerable, int size)
        {
            if (size <= 0)
            {
                throw new Exception("Parameter SIZE must be higher than zero");
            }
            var data = enumerable.InclusiveToList();
            int pages = (data.Count / size) + 1;
            for (int page = 0; page < pages; page++)
            {
                yield return data.Skip(page * size).Take(size);
            }
        }
        
        public static IEnumerable<List<T>> Batch<T>(this IQueryable<T> queryable, int size)
        {
            if (size <= 0)
            {
                throw new Exception("Parameter SIZE must be higher than zero");
            }
            int page = 0;
            List<T> data = new List<T>();
            do
            {
                data = queryable.Skip(page++ * size).Take(size).ToList();
                if (data.Any())
                {
                    yield return data;
                }
                else
                {
                    break;
                }
            } while (true);
        }
        
		public static bool ContainsAnyOf(this string str, params string[] list)
        {
            return list.Contains(str);
        }
        
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
        
        public static bool IsDevLikeEnvironment(this IHostingEnvironment environment)
        {
            return (environment.IsDevelopment() || environment.EnvironmentName.ToLowerInvariant().ContainsAnyOf("dev","test", "development"));
        }
		
        public static byte[] ConvertBase64ToRaw(string dataBase64)
        {
            return Convert.FromBase64String(dataBase64);
        }

        public static int ValueOrDefaultIfZero(this int integer, int defaultValue)
        {
            return integer > 0 ? integer : defaultValue;
        }
        
        
        public static DateTime Max(params DateTime[] dates) => dates.Max();
        
        public static string GetBearerToken(this IHttpContextAccessor ctxAccessor)
        {
            var authHeader = ctxAccessor?.HttpContext?.Request?.Headers["Authorization"];
            
            if (authHeader != default(StringValues?) && authHeader.HasValue && authHeader.Value != default(StringValues))
            {
                return string.Join("", authHeader).Split(' ').LastOrDefault() ?? string.Empty;
            }
            return null;
        }
        
        
        public static IPerformanceMonitorManager CopyPerformanceMonitoring(this IServiceProvider serviceProvider, IServiceProvider parent)
        {
            var monitor = (IPerformanceMonitorManager) serviceProvider.GetService(typeof(IPerformanceMonitorManager));
            monitor.Assign((IPerformanceMonitorManager)parent.GetService(typeof(IPerformanceMonitorManager)));
            return monitor;
        }

        public static string Affix(this string mainStr, string addStr)
        {
            return string.IsNullOrEmpty(addStr) ? mainStr : mainStr + addStr;
        }
        
        /// <summary>
        /// Combine many uris into one
        /// </summary>
        /// <param name="uris"></param>
        /// <returns></returns>
        public static string CombineUris(params string[] uris)
        {
            return string.Join('/', uris.Select(i => i.Trim('/')));
        }
        
        
        /// <summary>
        /// Return response with specific HTTP status code and object (json) as additional data
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ObjectResult ReturnStatusCode(int statusCode, object value)
        {
            return new ObjectResult(value)
            {
                StatusCode = new int?(statusCode)
            };
        }

        /// <summary>
        /// Return response with  HTTP 403 (forbidden) status code and object (json) as additional data
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ObjectResult ReturnStatusForbidden(object value)
        {
            return new ObjectResult(value)
            {
                StatusCode = 403
            };
        }

        /// <summary>
        /// Convert string representation of enum to enum type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="defaultValueIfNotFound"></param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(this string str, T defaultValueIfNotFound) where T : struct
        {
            return ((string.IsNullOrEmpty(str)) || (!Enum.TryParse(typeof(T), str, true, out object oResult))) ? defaultValueIfNotFound : (T) oResult;
        }


        /// <summary>
        /// Apply where clausule to filter out null references
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Where(i => i != null);
        }
        
        /// <summary>
        /// Convert string representation of enum to enum type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T? ConvertToEnum<T>(this string str) where T : struct
        {
            return ((string.IsNullOrEmpty(str)) || (!Enum.TryParse(typeof(T), str, true, out object oResult))) ? null : (T?)oResult;
        }

        /// <summary>
        /// Compares two strings and ignores culture and case 
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns></returns>
        public static bool Is(this string strA, string strB)
        {
            if (strA == null)
            {
                return strB == null;
            }
            return strA.Equals(strB, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Compares two strings and ignores culture and case 
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns></returns>
        public static bool Not(this string strA, string strB)
        {
            return !strA.Is(strB);
        }

        /// <summary>
        /// Call action only on object that is not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="action">Action that will be called on target object</param>
        public static void SafeCall<T>(this T target, Action<T> action) where T:  class
        {
            if (target == null) return;
            action(target);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="target"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TOut SafeCall<T, TOut>(this T target, Func<T, TOut> action) where T : class
        {
            if (target == null) return default(TOut);
            return action(target);
        }


        /// <summary>
        /// Get property of object from collection if this object is not null
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="enumerable">Collection that is searched</param>
        /// <param name="property">Value of this property will be returned</param>
        /// <returns>Value of property</returns>
        public static TResult SafePropertyFromFirst<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> property)
        {
            if (enumerable == null) return default(TResult);
            var instance = enumerable.FirstOrDefault();
            if (instance == null) return default(TResult);
            return property(instance);
        }

        /// <summary>
        /// Set value to property of target object
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TOutProperty"></typeparam>
        /// <param name="target">Target object which property will be set</param>
        /// <param name="memberLamda">Property selector</param>
        /// <param name="value">Value that will be set to property</param>
        public static void SetPropertyValue<TTarget, TOutProperty>(this TTarget target, Expression<Func<TTarget, TOutProperty>> memberLamda, object value)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression ?? ((UnaryExpression)memberLamda.Body).Operand as MemberExpression;

            if (memberSelectorExpression == null)
            {
                throw new Exception("Property selector cast error.");
            }

            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new Exception("Memberexpression cast error.");
            }

            property?.SetValue(target, value, null);
        }
       
        /// <summary>
        /// Get string name of property
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TOutProperty"></typeparam>
        /// <param name="target">Target object which property will be set</param>
        /// <param name="memberLamda">Property selector</param>
        public static string GetPropertyName<TTarget, TOutProperty>(this TTarget target, Expression<Func<TTarget, TOutProperty>> memberLamda)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression ?? ((UnaryExpression)memberLamda.Body).Operand as MemberExpression;
            if (memberSelectorExpression == null)
            {
                throw new Exception("Property selector cast error.");
            }
            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new Exception("Memberexpression cast error.");
            }
            return property.Name;
        }
        
        /// <summary>
        /// Get string name of property
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TOutProperty"></typeparam>
        /// <param name="memberLamda">Property selector</param>
        public static string GetPropertyName<TTarget, TOutProperty>(Expression<Func<TTarget, TOutProperty>> memberLamda)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression ?? ((UnaryExpression)memberLamda.Body).Operand as MemberExpression;
            if (memberSelectorExpression == null)
            {
                throw new Exception("Property selector cast error.");
            }
            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new Exception("Memberexpression cast error.");
            }
            return property.Name;
        }
        
        
        /// <summary>
        /// Add items to list from another list if they are not already present
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="addingList"></param>
        /// <returns></returns>
        public static List<T> AddWithDistinct<T>(this List<T> list, IEnumerable<T> addingList)
        {
            list.AddRange(addingList.Except(list));
            return list;
        }

        /// <summary>
        /// Creates lambda expression for equality from property name and value
        /// </summary>
        /// <typeparam name="T">Type on which the property will be used</typeparam>
        /// <param name="propertyName">Name of property</param>
        /// <param name="value">Value for comparison</param>
        /// <returns>Lambda expression</returns>
        public static Expression<Func<T, bool>> CreateLambdaEqual<T>(string propertyName, object value)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            Expression property = Expression.Property(parameter, propertyName);
            Expression target = Expression.Constant(value);
            Expression equalsMethod = Expression.Call(property, "Equals", null, target);
            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(equalsMethod, parameter);
            return lambda;
        }

        /// <summary>
        /// Set value to property of target object
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="target">Target object which property will be set</param>
        /// <param name="propertyName">Property name that should be set</param>
        /// <param name="value">Value that will be set to property</param>
        public static void SetPropertyValue<TTarget>(this TTarget target, string propertyName, object value)
        {
            if (target == null)
            {
                throw new ArgumentException("Calling SetPropertyValue on null instance.");
            }
            if (typeof(TTarget).GetProperty(propertyName) == null)
            {
               throw new ArgumentException("Invalid property name.");
            }
            typeof(TTarget).GetProperty(propertyName).SetValue(target, value, null);
        }


        /// <summary>
        /// Get value of property of target object
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="target">Target object which property will be get</param>
        /// <param name="propertyName">Property name that should be get</param>
        public static TReturn GetPropertyValue<TTarget, TReturn>(this TTarget target, string propertyName)
        {
            var propType = typeof(TTarget).GetProperty(propertyName);
            if (propType == null)
            {
                throw new ArgumentException("Property not found from object.");
            }
            if (!typeof(TReturn).IsAssignableFrom(propType.PropertyType))
            {
                return default(TReturn);
            }
            return (TReturn) propType.GetValue(target);
        }

        /// <summary>
        /// Get value of property of target object
        /// </summary>
        /// <param name="target">Target object which property will be get</param>
        /// <param name="propertyName">Property name that should be get</param>
        /// <returns>Object value of desired target and its property</returns>
        public static object GetPropertyObjectValue(this object target, string propertyName)
        {
            var property = target.GetType().GetProperty(propertyName);
            if (property == null)
            {
                throw new ArgumentException("Known invalid property name.");
            }
            return property.GetValue(target);
        }


        /// <summary>
        /// Checks if enumeration is empty. ie containing no value or it is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }
        
        /// <summary>
        /// Checks if enumeration has any data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool HasData<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.IsNullOrEmpty();
        }

        public static bool IsNullOrWhitespace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }
    
        /// <summary>
        /// Convert readonly list  to list. Converstion done by type checking, if fails, then it is converted by ToList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">Readonly list that will be converted</param>
        /// <returns>list of List type</returns>
        public static List<T> InclusiveToList<T>(this IEnumerable<T> enumerable)
        {
            return enumerable as List<T> ?? enumerable?.ToList();
        }

        /// <summary>
        /// Add item to collection and return it immediately
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">Target collection where item will be added</param>
        /// <param name="item">Item to Add</param>
        /// <returns>Added item</returns>
        public static T AddAndReturn<T>(this ICollection<T> collection, T item)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection", "Value cannot be null.");
            }
            collection.Add(item);
            return item;
        }

        /// <summary>
        /// Checks if target type is Enumerable type, i.e. any type that implements IEnumerable interface
        /// </summary>
        /// <param name="target">Target that will be check if it is IEnumerable</param>
        /// <param name="exceptions">List of Types that will be considered as exceptions, i.e. if type is in exception list, it will return False</param>
        /// <returns>True if type implements IEnumerable</returns>
        public static bool IsEnumerable(this Type target, List<Type> exceptions = null)
        {
            if (exceptions?.Contains(target) == true)
            {
                return false;
            }
            var checkedTypes = (target ?? throw new ArgumentNullException(nameof(target))).GetInterfaces().ToList();
            checkedTypes.Add(target);
            return checkedTypes.Any(intType => intType.GetTypeInfo().IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }


        public static bool IsCollection(this Type target)
        {
            var checkedTypes = (target ?? throw new ArgumentNullException(nameof(target))).GetInterfaces().ToList();
            checkedTypes.Add(target);
            return checkedTypes.Any(intType => intType.GetTypeInfo().IsGenericType && intType.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        /// <summary>
        /// Run action in separated thread and wait for its exit
        /// </summary>
        /// <param name="action">Action that will be called in thread</param>
        public static void RunInThreadAndWait(Action action)
        {
            if (action == null) return;
            Exception dbException = null;
            Thread workerThread = new Thread(() => action()) { IsBackground = false };
            workerThread.Start();
            workerThread.Join();
            if (dbException != null)
            {
                throw dbException;
            }
        }

        /// <summary>
        /// Guid extension checking if Guid is assigned, i.e. it is not null and not empty Guid
        /// </summary>
        /// <param name="guid">Guid that will be tested</param>
        /// <returns>True if valid Guid is assigned</returns>
        public static bool IsAssigned(this Guid? guid)
        {
            return guid.HasValue && guid.Value.IsAssigned();
        }

        /// <summary>
        /// Guid extension checking if Guid is assigned, i.e. it is not null and not empty Guid
        /// </summary>
        /// <param name="guid">Guid that will be tested</param>
        /// <returns>True if valid Guid is assigned</returns>
        public static bool IsAssigned(this Guid guid)
        {
            return guid != Guid.Empty;
        }

        /// <summary>
        /// Calculate Guid from string
        /// </summary>
        /// <param name="str">Input string that will be used as input for Guid calculation</param>
        /// <param name="forType">Type of entity for which the GUID will be generated, influences hash calcuation</param>
        /// <returns>Guid from string</returns>
        public static Guid GetGuid(this string str, string forType = null)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes((String.IsNullOrEmpty(forType) ? String.Empty : forType) + str));
                return new Guid(hash);
            }
        }

        /// <summary>
        /// Calculate Guid from string
        /// </summary>
        /// <typeparam name="ForType">Type of entity for which the GUID will be generated, influences hash calcuation</typeparam>
        /// <param name="str">Input string that will be used as input for Guid calculation</param>
        /// <returns>Guid from string</returns>
        public static Guid GetGuid<ForType>(this string str)
        {
            return GetGuid(str, typeof(ForType).Name);
        }

        /// <summary>
        /// Calculate Guid from string
        /// </summary>
        /// <param name="str">Input string that will be used as input for Guid calculation</param>
        /// <param name="forType">Type of entity for which the GUID will be generated, influences hash calcuation</param>
        /// <returns>Guid from string</returns>
        public static Guid GetGuid(this string str, Type forType)
        {
            return GetGuid(str, forType?.Name);
        }

        /// <summary>
        /// String extension parsing content into Guid
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Guid if string is Guid or null if it is invalid</returns>
        public static Guid? ParseToGuid(this string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Guid id;
            if (Guid.TryParse(str, out id) && id != Guid.Empty)
            {
                return id;
            }
            return null as Guid?;
        }

        /// <summary>
        /// Dictionary extension for trying get value by key, returns default value if does not exist
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary">Input dictionary</param>
        /// <param name="key">Key for selecting item from dictionary</param>
        /// <returns>Value from dictionary or type's default value if does not exist</returns>
        public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("Dictionary can not be null.");
            }
            return TryGetImpl(dictionary, key);
        }

        /// <summary>
        /// Dictionary extension for trying get value by key, returns default value if does not exist
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary">Input dictionary</param>
        /// <param name="key">Key for selecting item from dictionary</param>
        /// <returns>Value from dictionary or type's default value if does not exist</returns>
        public static TValue TryGet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("Dictionary can not be null.");
            }
            return TryGetImpl(dictionary, key);
        }

        /// <summary>
        /// Dictionary extension for trying get value by key, returns default value if does not exist
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary">Input dictionary</param>
        /// <param name="key">Key for selecting item from dictionary</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Value from dictionary or type's default value if does not exist</returns>
        public static TValue TryGetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            return TryGetImplWithDefault(dictionary, key, defaultValue);
        }
        
        /// <summary>
        /// Dictionary extension for trying get value by key, returns default value if does not exist
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary">Input dictionary</param>
        /// <param name="key">Key for selecting item from dictionary</param>
        /// <param name="defaultValueProvider">Default value if not found</param>
        /// <returns>Value from dictionary or type's default value if does not exist</returns>
        public static TValue TryGetOrCallDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider)
        {
            return TryGetImplWithDefault(dictionary, key, defaultValueProvider);
        }
        
        /// <summary>
        /// Dictionary extension for trying get value by key, returns default value if does not exist
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary">Input dictionary</param>
        /// <param name="key">Key for selecting item from dictionary</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Value from dictionary or type's default value if does not exist</returns>
        public static TValue TryGetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            return TryGetImplWithDefault(dictionary, key, defaultValue);
        }

        /// <summary>
        /// Dictionary extension for trying get value by key, returns default value if does not exist
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary">Input dictionary</param>
        /// <param name="key">Key for selecting item from dictionary</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Value from dictionary or type's default value if does not exist</returns>
        public static TValue TryGetOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            return TryGetImplWithDefault(dictionary, key, defaultValue);
        }

        /// <summary>
        /// Dictionary extension for trying get value by key, returns default value if does not exist
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary">Input dictionary</param>
        /// <param name="key">Key for selecting item from dictionary</param>
        /// <returns>Value from dictionary or type's default value if does not exist</returns>
        private static TValue TryGetImpl<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return TryGetImplWithDefault(dictionary, key, default(TValue));
        }

        /// <summary>
        /// Dictionary extension for trying get value by key, returns default value if does not exist
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary">Input dictionary</param>
        /// <param name="key">Key for selecting item from dictionary</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Value from dictionary or type's default value if does not exist</returns>
        private static TValue TryGetImplWithDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            return defaultValue;
        }
        
        private static TValue TryGetImplWithDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            return defaultValueProvider();
        }

        /// <summary>
        /// Read whole hierarchy of exceptions and put them and their message to target string
        /// </summary>
        /// <param name="excp">Exception that will be extracted</param>
        /// <returns>String containing all inner exceptions and their messages</returns>
        public static string ExtractAllInnerExceptions(Exception excp)
        {
            if (excp == null) return String.Empty;
            return excp.GetType().Name + " : " + excp.Message + Environment.NewLine + ExtractAllInnerExceptions(excp.InnerException);
        }

        /// <summary>
        /// Call specific action and repeat it specified times if exception occured or false returned
        /// </summary>
        /// <param name="retries">How many time the action should be repeated</param>
        /// <param name="action">Action that will be repeated if needed</param>
        public static void RunWithRetries(int retries, Func<bool> action)
        {
            if (retries <= 0)
            {
                throw new ArgumentException("Invalid value given for retries count.");
            }

            int tries = 0;
            while (tries++ < retries)
            {
                try
                {
                    if (action())
                    {
                        return;
                    }
                }
                catch (Exception)
                {}
            }
        }

        public static IEnumerable<T> Flatten<T, R>(this IEnumerable<T> source, Func<T, R> recursion) where R : IEnumerable<T>
        {
            var flattened = source.ToList();

            var children = source.Select(recursion);

            if (children == null) return flattened;

            foreach (var child in children)
            {
                flattened.AddRange(child.Flatten(recursion));
            }

            return flattened;
        }

        public static Stream Compress(this Stream decompressedStream, CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            var compressed = new MemoryStream();
            using (var zip = new GZipStream(compressed, compressionLevel, true))
            {
                decompressedStream.CopyTo(zip);
            }

            compressed.Seek(0, SeekOrigin.Begin);
            return compressed;
        }

        public static Stream Decompress(this Stream compressedStream)
        {
            var decompressed = new MemoryStream();
            using (var zip = new GZipStream(compressedStream, CompressionMode.Decompress, true))
            {
                zip.CopyTo(decompressed);
            }

            decompressed.Seek(0, SeekOrigin.Begin);
            return decompressed;
        }

        public static string SerializeXmlObject<T>(this T toSerialize)
        {
            if (toSerialize == null)
            {
                throw new ArgumentNullException("Object to serialize is null.");
            }
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static T DeserializeXmlObject<T>(this string toDeserialize)
        {
            if (toDeserialize == null)
            {
                throw new ArgumentNullException("String to deserialize is null.");
            }
            if (string.IsNullOrWhiteSpace(toDeserialize))
            {
                throw new ArgumentException("String to deserialize is empty or whitespaces.","toDeserialize");
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (StringReader textReader = new StringReader(toDeserialize))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }
        }

        public static string ToXmlString(this RSACryptoServiceProvider rsa)
        {
            RSAParameters parameters = rsa.ExportParameters(true);

            return String.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(parameters.Modulus),
                Convert.ToBase64String(parameters.Exponent),
                Convert.ToBase64String(parameters.P),
                Convert.ToBase64String(parameters.Q),
                Convert.ToBase64String(parameters.DP),
                Convert.ToBase64String(parameters.DQ),
                Convert.ToBase64String(parameters.InverseQ),
                Convert.ToBase64String(parameters.D));
        }

        public static void FromXmlString(this RSA rsa, string xmlString)
        {
            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = Convert.FromBase64String(node.InnerText); break;
                        case "Exponent": parameters.Exponent = Convert.FromBase64String(node.InnerText); break;
                        case "P": parameters.P = Convert.FromBase64String(node.InnerText); break;
                        case "Q": parameters.Q = Convert.FromBase64String(node.InnerText); break;
                        case "DP": parameters.DP = Convert.FromBase64String(node.InnerText); break;
                        case "DQ": parameters.DQ = Convert.FromBase64String(node.InnerText); break;
                        case "InverseQ": parameters.InverseQ = Convert.FromBase64String(node.InnerText); break;
                        case "D": parameters.D = Convert.FromBase64String(node.InnerText); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }

        public static int? ParseToInt(this string str)
        {
            int result;
            if (!Int32.TryParse(str, out result)) return null;
            return result;
        }

        public static double? ParseToDouble(this string str)
        {
            double result;
            if (!Double.TryParse(str, out result)) return null;
            return result;
        }

        private static string CheckPath(Func<string, bool> check, IHostingEnvironment env, string devRootDirectory, string path)
        {
            var pathFirst = Path.Combine(env.ContentRootPath, path);
            var pathSecond = Path.Combine(devRootDirectory, path);
            var pathThird = path;
            return check(pathFirst) ? pathFirst : check(pathSecond) ? pathSecond : pathThird;
        }

        public static string GetFilePath(this IHostingEnvironment env, string devRootDirectory, string fileName)
        {
            return CheckPath(File.Exists, env, devRootDirectory, fileName);
        }
        
        public static string GetDirectoryPath(this IHostingEnvironment env, string devRootDirectory, string directoryName)
        {
            return CheckPath(Directory.Exists, env, devRootDirectory, directoryName);
        }

        /// <summary>
        /// Parses to unique identifier with exeption.
        /// </summary>
        /// <param name="stringGuid">The string unique identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static Guid ParseToGuidWithExeption(this string stringGuid)
        {
            Guid? guid = stringGuid.ParseToGuid();
            if (guid == null)
            {
                throw new Exception($"Cannot parse '{stringGuid}' to type of Guid.");
            }
            return guid.Value;
        }

        /// <summary>
        /// Parses the specified enum value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.Exception"></exception>
        public static TEnum Parse<TEnum>(this string enumValue)
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum) throw new PtvArgumentException(CoreMessages.OpenApi.NotEnum);

            try
            {
                return (TEnum)Enum.Parse(typeof(TEnum), enumValue);
            }
            catch(Exception)
            {
                throw new Exception(String.Format(CoreMessages.OpenApi.RequestMalFormatted, typeof(TEnum).Name, GetEnumvaluesAsList<TEnum>()));
            }
        }

        private static string GetEnumvaluesAsList<TEnum>()
        {
            return String.Join(", ", Enum.GetNames(typeof(TEnum)));
        }


        public static string ConvertToString(this TimeSpan timeSpan)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int) timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
        }


        public static int PositiveOrZero(this int integer)
        {
            return integer > 0 ? integer : 0;
        }

        /// <summary>
        /// Helper to check if the are more results available (used when results are paged).
        /// </summary>
        /// <param name="totalResults">How many results there are</param>
        /// <param name="pageNumber">page number of the results (first page is assumed to be number 0)</param>
        /// <param name="pageSize">how many items are on per page</param>
        /// <returns>true if there are more "page" results left (totalResults > pagesize * pagenumber) otherwise false</returns>
        /// <remarks>
        /// <para>If <paramref name="totalResults"/> is less than one returns false.</para>
        /// <para>If <paramref name="pageNumber"/> is less than zero returns false</para>
        /// <para>If <paramref name="pageSize"/> is less than one returns false</para>
        /// </remarks>
        public static bool MoreResultsAvailable(this int totalResults, int pageNumber, int pageSize = CoreConstants.MaximumNumberOfAllItems)
        {
            bool moreAvailable = false;

            if (totalResults > 0 && pageNumber >= 0 && pageSize > 0)
            {
                if (pageNumber == 0)
                {
                    // first page
                    moreAvailable = totalResults > pageSize;
                }
                else
                {
                    // page number used as zero indexed so just add one to it to be able to calculate correctly results left (++pagenumber add one before multiplying)
                    moreAvailable = totalResults > ++pageNumber * pageSize;
                }
            }

            return moreAvailable;
        }

        // Convert the string to camel case.
        public static string ToCamelCase(this Enum enumVar)
        {
            var stringEnum = enumVar.ToString();
            // If there are 0 or 1 characters, just return the string.
            if (stringEnum == null || stringEnum.Length < 2)
                return stringEnum;

            // Split the string into words.
            var words = stringEnum.Split('_');
            
            // Combine the words.
            var result = char.ToLowerInvariant(words[0][0]) + words[0].Substring(1);
            for (var i = 1; i < words.Length; i++)
            {
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1);
            }

            return result;
        }

        public static void RemoveWhere<T>(this IList<T> list, Func<T, bool> condition)
        {
            var toRemove = list.Where(condition).ToList();
            toRemove.ForEach(i => list.Remove(i));
        }

        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dictA, IDictionary<TKey, TValue> dictB)
        {
            return dictA.Keys.Union(dictB.Keys).Distinct().ToDictionary(k => k, k => dictA.ContainsKey(k) ? dictA[k] : dictB[k]);
        }
        
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictA, Dictionary<TKey, TValue> dictB)
        {
            return dictA.Keys.Union(dictB.Keys).Distinct().ToDictionary(k => k, k => dictA.ContainsKey(k) ? dictA[k] : dictB[k]);
        }

        public static Dictionary<TKey, Tuple<TValue, TValue>> Cross<TKey, TValue>(this IDictionary<TKey, TValue> dicA, IDictionary<TKey, TValue> dicB)
        {
            return dicA.Keys.Where(dicB.ContainsKey).ToDictionary(k => k, k => new Tuple<TValue , TValue>( dicA[k], dicB[k] ));
        }
        
        public static Dictionary<TKey, Tuple<TValue, TValue>> Cross<TKey, TValue>(this Dictionary<TKey, TValue> dicA, Dictionary<TKey, TValue> dicB)
        {
            return dicA.Keys.Where(dicB.ContainsKey).ToDictionary(k => k, k => new Tuple<TValue , TValue>( dicA[k], dicB[k] ));
        }

        public static string ConvertToString(this IList<Guid> guidList)
        {
            return String.Join(", ", guidList);
        }

        public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .Select(s => s.Trim())
                .ToList();
        }

        public static string GetSha256Hash(this string str)
        {
            return str.GetSha256Hash(Encoding.Unicode);
        }

        public static string GetSha256Hash(this string str, Encoding encoding, bool convertToBase64String = true)
        {
            using (var sha = SHA256.Create())
            {
                var inArray = sha.ComputeHash(encoding.GetBytes(str));
                return convertToBase64String
                    ? Convert.ToBase64String(inArray)
                    : FromByteArrayToString(inArray);
            }
        }
        
        private static string FromByteArrayToString(byte[] hash)
        {
            var result = new StringBuilder();
            foreach (byte t in hash)
            {
                result.Append(t.ToString("X2"));
            }
            return result.ToString().ToUpper();
        }

        public static IEnumerable<T> DistinctBy<T>(this IEnumerable<T> enumerable, params Func<T, object>[] compareBy)
        {
            return enumerable.Distinct(new DistinctEqualityComparer<T>(compareBy));
        }

        public static string SafeSubstring(this string input, int start, int length)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            
            if (start < 0 || length < 0)
                return null;

            if (start > input.Length)
                return null;
            
            var substringEnd = Math.Min(length, input.Length - start);
            return input.Substring(start, substringEnd);
        }

        public static string WithQuotes(this string input, string quoteType = "\"")
        {
            return quoteType + input + quoteType;
        }
        
        private class DistinctEqualityComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, object>[] compareByLambdas;

            public DistinctEqualityComparer(Func<T, object>[] compareBy)
            {
                if (!compareBy.Any())
                {
                    throw new Exception("Specify at least one lambda for equality comparison.");
                }
                this.compareByLambdas = compareBy;
            }

            public bool Equals(T x, T y)
            {
                foreach (var compareByLambda in compareByLambdas)
                {
                    var valueOfX = compareByLambda(x);
                    var valueOfY = compareByLambda(y);
                    if (!((valueOfX == valueOfY) || (valueOfX != null && valueOfX.Equals(valueOfY)) || ((valueOfX != null && valueOfY != null) && (valueOfX.GetHashCode() == valueOfY.GetHashCode()))))
                    {
                        return false;
                    }
                }
                return true;
            }

            public int GetHashCode(T obj)
            {
                return compareByLambdas.Select(compareByLambda => compareByLambda(obj)?.GetHashCode() ?? 0).Aggregate((i,j) => i+j);
            }
        }

    }
}
