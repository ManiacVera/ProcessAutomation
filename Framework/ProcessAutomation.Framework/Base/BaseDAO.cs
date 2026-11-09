using Microsoft.EntityFrameworkCore;
using ProcessAutomation.Framework.DTO.Constants;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.DTO.Interface;
using ProcessAutomation.Framework.DTO.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ProcessAutomation.Framework.Base
{
	public abstract class BaseDAO<T_CONTEXT> where T_CONTEXT : DbContext
	{
		protected T_CONTEXT context;
		public delegate T ProcessCallBackDAO<T>(T_CONTEXT context);

		public BaseDAO()
		{
			SetContext();
		}

		protected abstract void SetContext();

		protected T_CONTEXT GetContext()
		{
			return context;
		}

		public ResponseInfo<T> ProcessDAO<T>(ProcessCallBackDAO<T> procesCallBack)
		{
			ResponseInfo<T> responseInfo = new ResponseInfo<T>();
			try
			{
				T_CONTEXT context = GetContext();
				if (procesCallBack != null)
				{
					responseInfo.Data = procesCallBack(context);
					responseInfo.Response = ServiceConstants.OK;
				}
			}
			catch (Exception ex)
			{
				responseInfo.Error = ex.Message;
				responseInfo.Data = default(T);
			}
			return responseInfo;
		}

		protected DbSet<T> GetEntity<T>(T entity) where T : class
		{
			return GetContext().Set<T>();
		}

		public ResponseInfo<List<T>> CreateRegeditByCollection<T>(List<T> entityList) where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				context.AddRange(entityList);
				context.SaveChanges();
				return entityList;
			});
		}

		private void SetCommonProperties<T>(T entity)
		{
			if (entity.GetType().BaseType == typeof(COMMON_FIELD))
			{
				PropertyInfo[] propertyArray = typeof(COMMON_FIELD).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				PropertyInfo[] entityPropertyArray = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

				if (entityPropertyArray != null)
				{
					foreach (PropertyInfo property in propertyArray)
					{
						PropertyInfo entityProperty = entityPropertyArray.FirstOrDefault(f => f.Name == property.Name);

						if (entityProperty != null)
						{
							switch (property.Name)
							{
								case FrameworkConstants.ALTERATION_COUNTER:
									property.SetValue(entity, ((int)property.GetValue(entity, null)) + 1, null);
									break;
								case FrameworkConstants.CREATION_DATE:
									object dateValue = property.GetValue(entity, null);
									if (dateValue != null && (DateTime)dateValue == default(DateTime))
									{
										property.SetValue(entity, DateTime.Now, null);
									}
									break;
								case FrameworkConstants.MODIFICATION_DATE:
									property.SetValue(entity, DateTime.Now, null);
									break;
							}
						}
					}
				}
			}
		}

		public ResponseInfo<T> CreateRegedit<T>(T entity) where T : class
		{
			return ProcessDAO<T>((context) => {
				// context.Set<T>().Add(entity);
				SetCommonProperties(entity);
				context.Entry(entity).State = EntityState.Added;
				context.SaveChanges();
				return entity;
			});
		}

		public ResponseInfo<T> PersistRegedit<T, KValue>(T entity) where T : class
		{
			ResponseInfo<T> result = null;

			string nameValue = GetValueFromAttribute<string>(entity, FrameworkConstants.NAME_FIELD_ATTRIBUTE);
			KValue keyValue = GetValueFromAttribute<KValue>(entity, FrameworkConstants.KEY_ATTRIBUTE);

			Expression<Func<T, bool>> lambdaExpressionLeft = GetPredicateByString<T, string>(FrameworkConstants.NAME_FIELD_ATTRIBUTE, nameValue);
			Expression<Func<T, bool>> lambdaExpressionRight = GetPredicateByString<T, KValue>(FrameworkConstants.KEY_ATTRIBUTE, keyValue);

			if (!ExistsRegedit<T>(ExpressionAndAlso<T>(lambdaExpressionLeft, lambdaExpressionRight)))
			{
				result = CreateRegedit<T>(entity);
			}
			else
			{
				result = UpdateRegedit<T>(entity);
			}

			return result;
		}

		public ResponseInfo<T> PersistRegedit<T>(T entity) where T : class
		{
			ResponseInfo<T> result = null;

			string nameValue = GetValueFromAttribute<string>(entity, FrameworkConstants.NAME_FIELD_ATTRIBUTE);
			string nameProperty = GetNameFromAttribute(entity, FrameworkConstants.NAME_FIELD_ATTRIBUTE);

			int keyValue = GetValueFromAttribute<int>(entity, FrameworkConstants.KEY_ATTRIBUTE);
			string keyProperty = GetNameFromAttribute(entity, FrameworkConstants.KEY_ATTRIBUTE);

			Expression<Func<T, bool>> lambdaExpressionLeft = GetPredicateByString<T, string>(nameProperty, nameValue);
			Expression<Func<T, bool>> lambdaExpressionRight = GetPredicateByString<T, int>(keyProperty, keyValue, "NotEqual");

			if (keyValue == 0 && !ExistsRegedit<T>(lambdaExpressionLeft))
			{
				result = CreateRegedit<T>(entity);
			}
			else if (keyValue != 0 && !ExistsRegedit<T>(ExpressionAndAlso(lambdaExpressionLeft, lambdaExpressionRight)))
			{
				result = UpdateRegedit<T>(entity);
			}

			return result;
		}

		public ResponseInfo<List<T>> UpdateRegeditByCollection<T>(List<T> entityList) where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				context.UpdateRange(entityList);
				context.SaveChanges();
				return entityList;
			});
		}

		public ResponseInfo<T> UpdateRegedit<T>(T entity) where T : class
		{
			return ProcessDAO<T>((context) => {
				SetCommonProperties(entity);
				context.Entry(entity).State = EntityState.Modified;
				context.SaveChanges();
				return entity;
			});
		}

		public ResponseInfo<T> UpdateRegedit<T>(T entity, Expression<Func<T, bool>> expression) where T : class
		{
			return ProcessDAO<T>((context) => {
				ResponseInfo<T> entityFound = RetrieveFirstOrDefaultByExpression<T>(expression);
				if (entityFound != null && !entityFound.HasError() && entityFound.Data != null)
				{
					context.Entry(entityFound.Data).State = EntityState.Modified;
					context.SaveChanges();
				}
				return entityFound.Data;
			});
		}

		public ResponseInfo<bool> DeleteRegeditByCollection<T>(List<T> entityList) where T : class
		{
			return ProcessDAO<bool>((context) => {
				context.RemoveRange(entityList);
				context.SaveChanges();
				return true;
			});
		}

		public ResponseInfo<bool> DeleteRegedit<T>(T entity) where T : class
		{
			return ProcessDAO<bool>((context) => {
				context.Attach(entity);
				context.Remove(entity);
				context.SaveChanges();
				return true;
			});
		}

		public ResponseInfo<bool> DeleteRegedit<T>(Expression<Func<T, bool>> expression) where T : class
		{
			return ProcessDAO<bool>((context) => {
				bool retorno = false;
				ResponseInfo<List<T>> entityFound = RetrieveListByExpression(expression);
				if (entityFound != null && !entityFound.HasError() && entityFound.Data != null)
				{
					foreach (T entity in entityFound.Data)
					{
						DbSet<T> dynamicContext = context.Set<T>();
						dynamicContext.Attach(entity);
						dynamicContext.Remove(entity);
						context.SaveChanges();
					}
					retorno = true;
				}
				return retorno;
			});
		}

		private IQueryable<T> IncludePopulate<T>(DbSet<T> entity, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			IQueryable<T> dynamicEntity = null;
			if (includeExpressions.Any())
			{
				dynamicEntity = includeExpressions.Aggregate<Expression<Func<T, object>>, IQueryable<T>>
				(entity, (current, expression) => current.Include(expression));
			}
			return dynamicEntity;
		}

		public ResponseInfo<List<T>> RetrieveList<T>() where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				return context.Set<T>().ToList();
			});
		}

		public ResponseInfo<List<T>> RetrieveListNoTracking<T>() where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				return context.Set<T>().AsNoTracking().ToList();
			});
		}

		public ResponseInfo<List<T>> RetrieveList<T>(params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				return IncludePopulate<T>(context.Set<T>(), includeExpressions)?.ToList();
			});
		}

		public ResponseInfo<List<T>> RetrieveListByExpression<T>(Expression<Func<T, bool>> expression) where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				return context.Set<T>().Where(expression).ToList();
			});
		}

		public ResponseInfo<List<T>> RetrieveListByExpressionNoTracking<T>(Expression<Func<T, bool>> expression) where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				return context.Set<T>().AsNoTracking().Where(expression).ToList();
			});
		}

		public ResponseInfo<List<T>> RetrieveListByExpression<T>(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				return IncludePopulate<T>(context.Set<T>(), includeExpressions)?.Where(expression).ToList();
			});
		}

		public ResponseInfo<List<T>> RetrieveListInAscendingByExpression<T, TORDER>(Expression<Func<T, TORDER>> order
			, Expression<Func<T, bool>> expression = null) where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				if (expression != null)
					return context.Set<T>().Where(expression).OrderBy(order).ToList();
				else
					return context.Set<T>().OrderBy(order).ToList();
			});
		}

		public ResponseInfo<List<T>> RetrieveListInAscendingByExpressionNoTracking<T, TORDER>(Expression<Func<T, TORDER>> order
			, Expression<Func<T, bool>> expression = null) where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				if (expression != null)
					return context.Set<T>().AsNoTracking().Where(expression).OrderBy(order).ToList();
				else
					return context.Set<T>().AsNoTracking().OrderBy(order).ToList();
			});
		}

		public ResponseInfo<List<T>> RetrieveListInAscendingByExpression<T, TORDER>(Expression<Func<T, TORDER>> order
			, Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				if (expression != null)
					return IncludePopulate<T>(context.Set<T>(), includeExpressions)?.Where(expression).OrderBy(order).ToList();
				else
					return IncludePopulate<T>(context.Set<T>(), includeExpressions)?.OrderBy(order).ToList();
			});
		}

		public ResponseInfo<List<T>> RetrieveListInDescendingByExpression<T, TORDER>(Expression<Func<T, TORDER>> order
			, Expression<Func<T, bool>> expression = null) where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				if (expression != null)
					return context.Set<T>().Where(expression).OrderByDescending(order).ToList();
				else
					return context.Set<T>().OrderByDescending(order).ToList();
			});
		}

		public ResponseInfo<List<T>> RetrieveListInDescendingByExpressionNoTracking<T, TORDER>(Expression<Func<T, TORDER>> order
			, Expression<Func<T, bool>> expression = null) where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				if (expression != null)
					return context.Set<T>().AsNoTracking().Where(expression).OrderByDescending(order).ToList();
				else
					return context.Set<T>().AsNoTracking().OrderByDescending(order).ToList();
			});
		}

		public ResponseInfo<List<T>> RetrieveListInDescendingByExpression<T, TORDER>(Expression<Func<T, TORDER>> order
			, Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				if (expression != null)
					return IncludePopulate<T>(context.Set<T>(), includeExpressions)?.Where(expression).OrderByDescending(order).ToList();
				else
					return IncludePopulate<T>(context.Set<T>(), includeExpressions)?.OrderByDescending(order).ToList();
			});
		}

		public ResponseInfo<T> RetrieveFirstOrDefaultByExpression<T>(Expression<Func<T, bool>> expression) where T : class
		{
			return ProcessDAO<T>((context) => {
				return context.Set<T>().FirstOrDefault(expression);
			});
		}

		public ResponseInfo<T> RetrieveFirstOrDefaultByExpressionNoTracking<T>(Expression<Func<T, bool>> expression) where T : class
		{
			return ProcessDAO<T>((context) => {
				return context.Set<T>().AsNoTracking().FirstOrDefault(expression);
			});
		}

		public ResponseInfo<T> RetrieveFirstOrDefaultByExpression<T>(Expression<Func<T, bool>> expression
			, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return ProcessDAO<T>((context) => {
				return IncludePopulate<T>(context.Set<T>(), includeExpressions)?.FirstOrDefault(expression);
			});
		}

		public bool ExistsRegedit<T>(Expression<Func<T, bool>> expression) where T : class
		{
			return ProcessDAO<bool>((context) => {
				bool retorno = false;
				ResponseInfo<T> entityFound = RetrieveFirstOrDefaultByExpression<T>(expression);
				if (entityFound != null && !entityFound.HasError() && entityFound.Data != null)
				{
					retorno = true;
				}
				return retorno;
			}).Data;
		}

		public ResponseInfo<List<T>> ExecuteStoreProcedure<T>(string storeProcedure, object[] parameters) where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				return context.Set<T>().FromSqlRaw<T>(storeProcedure, parameters).ToList();
			});
		}

		public Expression<Func<T, bool>> ExpressionAndAlso<T>(Expression<Func<T, bool>> expressionLeft, Expression<Func<T, bool>> expressionRight)
		{
			// need to detect whether they use the same
			// parameter instance; if not, they need fixing
			Expression<Func<T, bool>> expressionResult = null;

			ParameterExpression param = expressionLeft.Parameters[0];
			if (ReferenceEquals(param, expressionRight.Parameters[0]))
			{
				// simple version
				expressionResult = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expressionLeft.Body, expressionRight.Body), param);
			}
			else
			{
				// otherwise, keep expressionLeft "as is" and invoke expressionRight
				expressionResult = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expressionLeft.Body, Expression.Invoke(expressionRight, param)), param);
			}

			return expressionResult;
		}

		private Expression<Func<T, bool>> GetPredicateByString<T, TValue>(string propertyName, TValue value, string method = FrameworkConstants.EQUAL_PASCAL)
		{
			//const string propertyFormat = "<{0}>k__BackingField";
			Type itemType = typeof(T);
			ParameterExpression parameter = Expression.Parameter(itemType, "p");
			Expression expressionLeft = Expression.Property(parameter, itemType.GetProperty(propertyName, BindingFlags.Instance |
					   BindingFlags.Static |
					   BindingFlags.NonPublic |
					   BindingFlags.Public));
			Expression expressionRight = Expression.Constant(value, typeof(TValue));
			Expression expression = null;
			LambdaExpression lambdaExpression = null;
			Func<T, bool> function = null;

			switch (method)
			{
				case FrameworkConstants.EQUAL_PASCAL:
					{
						expression = Expression.Equal(expressionLeft, expressionRight);
						lambdaExpression = Expression.Lambda(expression, new[] { parameter });
						function = (Func<T, bool>)lambdaExpression.Compile();
					}
					break;
				case FrameworkConstants.NOT_EQUAL_PASCAL:
					{
						expression = Expression.NotEqual(expressionLeft, expressionRight);
						lambdaExpression = Expression.Lambda(expression, new[] { parameter });
						function = (Func<T, bool>)lambdaExpression.Compile();
					}
					break;
				case FrameworkConstants.CONTAINS_PASCAL:
					{
						MethodInfo methodInfo = value.GetType().GetMethods().FirstOrDefault(g => g.Name == FrameworkConstants.CONTAINS_PASCAL);
						expression = Expression.Call(expressionLeft, methodInfo, expressionRight);
						lambdaExpression = Expression.Lambda(expression, new[] { parameter });
						function = (Func<T, bool>)lambdaExpression.Compile();
					}
					break;
				default:
					{
						expression = Expression.Equal(expressionLeft, expressionRight);
						lambdaExpression = Expression.Lambda(expression, new[] { parameter });
						function = (Func<T, bool>)lambdaExpression.Compile();
					}
					break;
			}

			//return new Predicate<T>(function);
			return (Expression<Func<T, bool>>)lambdaExpression;
		}

		private TValue GetValueFromAttribute<TValue>(object entity, string attribute)
		{
			TValue value = default(TValue);
			foreach (PropertyInfo propertyInfo in entity.GetType().GetProperties())
			{
				if (propertyInfo.GetCustomAttributes().Any(f => f.GetType().Name == attribute))
				{
					var propertyValue = propertyInfo.GetValue(entity, null);
					if (propertyValue != null)
					{
						value = (TValue)propertyValue;
					}

					break;
				}
			}
			return value;
		}

		private string GetNameFromAttribute(object entity, string attribute)
		{
			string propertyName = null;
			foreach (PropertyInfo propertyInfo in entity.GetType().GetProperties())
			{
				if (propertyInfo.GetCustomAttributes().Any(f => f.GetType().Name == attribute))
				{
					propertyName = propertyInfo.Name;
					break;
				}
			}
			return propertyName;
		}

		private string GetPropertyNameFromAttribute<T>(string attribute)
		{
			string propertyName = null;
			foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
			{
				var prop = propertyInfo.GetCustomAttributes().ToList();
				if (propertyInfo.GetCustomAttributes().Any(f => f.GetType().Name == attribute))
				{
					propertyName = propertyInfo.Name;
					break;
				}
			}
			return propertyName;
		}

		protected ResponseInfo<T> RetrieveByAttribute<T, TValue>(string attribute, TValue value, string method = FrameworkConstants.EQUAL_PASCAL, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return ProcessDAO<T>((context) => {
				var expression = (Func<T, bool>)GetPredicateByString<T, TValue>(GetPropertyNameFromAttribute<T>(attribute), value, method).Compile();

				if (includeExpressions != null && includeExpressions.Any())
				{
					return IncludePopulate<T>(context.Set<T>(), includeExpressions)?.FirstOrDefault(expression);
				}
				else
				{
					return context.Set<T>().FirstOrDefault(expression);
				}
			});
		}

		protected ResponseInfo<List<T>> RetrieveListByAttribute<T, TValue>(string attribute, TValue value, string method = FrameworkConstants.EQUAL_PASCAL, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return ProcessDAO<List<T>>((context) => {
				var expression = (Func<T, bool>)GetPredicateByString<T, TValue>(GetPropertyNameFromAttribute<T>(attribute), value, method).Compile();

				if (includeExpressions != null && includeExpressions.Any())
				{
					return IncludePopulate<T>(context.Set<T>(), includeExpressions)?.Where(expression).ToList();
				}
				else
				{
					return context.Set<T>().Where(expression).ToList();
				}
			});
		}

		public ResponseInfo<T> RetrieveByKey<T>(int identifier, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return RetrieveByAttribute<T, int>(FrameworkConstants.KEY_ATTRIBUTE, identifier, FrameworkConstants.EQUAL_PASCAL, includeExpressions);
		}

		public ResponseInfo<T> RetrieveByIdentifier<T>(int identifier, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return RetrieveByAttribute<T, int>(FrameworkConstants.IDENTIFIER_FIELD_ATTRIBUTE, identifier, FrameworkConstants.EQUAL_PASCAL, includeExpressions);
		}

		public ResponseInfo<T> RetrieveByIdentifier<T, TValue>(TValue value, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return RetrieveByAttribute<T, TValue>(FrameworkConstants.IDENTIFIER_FIELD_ATTRIBUTE, value, FrameworkConstants.EQUAL_PASCAL, includeExpressions);
		}

		public ResponseInfo<T> RetrieveByName<T>(string value, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return RetrieveByAttribute<T, string>(FrameworkConstants.NAME_FIELD_ATTRIBUTE, value, FrameworkConstants.EQUAL_PASCAL, includeExpressions);
		}

		public ResponseInfo<T> RetrieveByCode<T>(string code, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return RetrieveByAttribute<T, string>(FrameworkConstants.CODE_FIELD_ATTRIBUTE, code, FrameworkConstants.EQUAL_PASCAL, includeExpressions);
		}

		public ResponseInfo<T> RetrieveByNumber<T>(int number, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return RetrieveByAttribute<T, int>(FrameworkConstants.NUMBER_FIELD_ATTRIBUTE, number, FrameworkConstants.EQUAL_PASCAL, includeExpressions);
		}

		public ResponseInfo<T> RetrieveByDescription<T>(string code, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return RetrieveByAttribute<T, string>(FrameworkConstants.DESCRIPTION_FIELD_ATTRIBUTE, code, FrameworkConstants.EQUAL_PASCAL, includeExpressions);
		}

		public ResponseInfo<T> RetrieveLikeDescription<T>(string code, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return RetrieveByAttribute<T, string>(FrameworkConstants.DESCRIPTION_FIELD_ATTRIBUTE, code, FrameworkConstants.CONTAINS_PASCAL, includeExpressions);
		}

		public ResponseInfo<List<T>> RetrieveListLikeDescription<T>(string code, params Expression<Func<T, object>>[] includeExpressions) where T : class
		{
			return RetrieveListByAttribute<T, string>(FrameworkConstants.DESCRIPTION_FIELD_ATTRIBUTE, code, FrameworkConstants.CONTAINS_PASCAL, includeExpressions);
		}

		protected ResponseInfo<int> RetrieveIdentifierByCodeBase<T_INFO, T_DAO>(string code, string propertyName, T_DAO dataObjectTransfer)
																			where T_DAO : IRetrieveByCode<T_INFO, string>
		{
			ResponseInfo<int> responseInfoIdentifier = new ResponseInfo<int>();
			ResponseInfo<T_INFO> responseInfoStatus = null;

			if ((responseInfoStatus = dataObjectTransfer.RetrieveByCode(code)) != null)
			{
				if (!responseInfoStatus.HasError())
				{
					responseInfoIdentifier.Response = responseInfoStatus.Response;
					responseInfoIdentifier.Data = (int)responseInfoStatus.Data.GetType().GetProperty(propertyName).GetValue(responseInfoStatus.Data, null);
				}
			}

			return responseInfoIdentifier;
		}

		protected ResponseInfo<int> RetrieveIdentifierByNumberBase<T_INFO, T_DAO>(int number, string propertyName, T_DAO dataObjectTransfer)
			where T_DAO : IRetrieveByNumber<T_INFO, int>
		{
			ResponseInfo<int> responseInfoIdentifier = new ResponseInfo<int>();
			ResponseInfo<T_INFO> responseInfoStatus = null;

			if ((responseInfoStatus = dataObjectTransfer.RetrieveByNumber(number)) != null)
			{
				if (!responseInfoStatus.HasError())
				{
					responseInfoIdentifier.Response = responseInfoStatus.Response;
					responseInfoIdentifier.Data = (int)responseInfoStatus.Data.GetType().GetProperty(propertyName).GetValue(responseInfoStatus.Data, null);
				}
			}

			return responseInfoIdentifier;
		}

		protected ResponseInfo<string> RetrieveCodeByIdentifierBase<T_INFO, T_DAO>(int idCode, string propertyName, T_DAO dataObjectTransfer)
																			where T_DAO : IRetrieveByIdentifier<T_INFO, int>
		{
			ResponseInfo<string> responseInfoCode = new ResponseInfo<string>();
			ResponseInfo<T_INFO> responseInfoStatus = null;

			if ((responseInfoStatus = dataObjectTransfer.RetrieveByIdentifier(idCode)) != null)
			{
				if (!responseInfoStatus.HasError())
				{
					responseInfoCode.Response = responseInfoStatus.Response;
					responseInfoCode.Data = (string)responseInfoStatus.Data.GetType().GetProperty(propertyName).GetValue(responseInfoStatus.Data, null);
				}
			}

			return responseInfoCode;
		}

		protected bool DetachTable<T>(T entity) where T : class
		{
			return ProcessDAO<bool>((context) => {
				context.Entry(entity).State = EntityState.Detached;
				return true;
			}).Data;
		}

		protected bool DetachTableList<T>(List<T> entityList) where T : class
		{
			return ProcessDAO<bool>((context) => {
				foreach(var entity in entityList)
				{
					context.Entry(entity).State = EntityState.Detached;
				}
				return true;
			}).Data;
		}
	}
}
