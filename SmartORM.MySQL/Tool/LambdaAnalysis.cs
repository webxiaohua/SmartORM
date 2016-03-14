using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Linq.Expressions;
using System.Reflection;

namespace SmartORM.MySQL.Tool
{
    /// <summary>
    /// 拉姆达表达式解析
    /// </summary>
    public class LambdaExpressionAnalysis
    {
        public enum MemberType
        {
            None = 0,
            Key = 1,
            Value = 2
        }
        private int SameIndex = 1;
        public string SqlWhere = null;
        public List<MySqlParameter> Params = new List<MySqlParameter>();

        public void AnalysisExpression(LambdaExpressionAnalysis lambda, Expression exp)
        {
            LambdaExpressionAnalysis.MemberType type = LambdaExpressionAnalysis.MemberType.None;
            this.SqlWhere = string.Format(" AND {0}", lambda.CreateSqlElements(exp, ref type));
        }

        /// <summary>
        /// 递归解析表达式计算
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="type"></param>
        /// <param name="isTrue"></param>
        /// <returns></returns>
        public string CreateSqlElements(Expression exp, ref MemberType type, bool isTrue = true)
        {
            if (exp is LambdaExpression)
            {
                LambdaExpression lambda = exp as LambdaExpression;
                var expression = lambda.Body;
                MemberType eleType = MemberType.None;
                return CreateSqlElements(expression, ref eleType);
            }
            else if (exp is BinaryExpression)
            {
                var expression = exp as BinaryExpression;
                MemberType leftType = MemberType.None;
                MemberType rightType = MemberType.None;
                var left = CreateSqlElements(expression.Left, ref leftType);
                var right = CreateSqlElements(expression.Right, ref rightType);
                var oper = GetOperator(expression.NodeType);
                if (leftType == MemberType.Key && rightType == MemberType.Value)
                {
                    var oldLeft = AddParas(ref left, right);
                    return string.Format(" ({0} {1} @{2}) ", oldLeft, oper, left);
                }
                else if (leftType == MemberType.Value && rightType == MemberType.Key)
                {
                    var oldLeft = AddParas(ref right, left);
                    return string.Format("( @{0} {1} {0} )", oldLeft, oper, right);
                }
                else if (leftType == MemberType.Value && rightType == MemberType.Value)
                {
                    return string.Format("( '{0}' {1} '{2}' )", left, oper, right);
                }
                else
                {
                    return string.Format("( {0} {1} {2} )", left, oper, right);
                }
            }
            else if (exp is BlockExpression)
            {

            }
            else if (exp is ConditionalExpression)
            {

            }
            else if (exp is MethodCallExpression)
            {
                MethodCallExpression mce = (MethodCallExpression)exp;
                string methodName = mce.Method.Name;
                if (methodName == "Contains")
                {
                    return Contains(methodName, mce, isTrue);
                }
                else if (methodName == "StartsWith")
                {
                    return StartsWith(methodName, mce, isTrue);
                }
                else if (methodName == "EndWith")
                {
                    return EndWith(methodName, mce, isTrue);
                }
                else if (methodName == "ToString")
                {
                    type = MemberType.Value;
                    return MethodToString(methodName, mce, ref type);
                }
                else if (methodName.StartsWith("To"))
                {
                    type = MemberType.Value;
                    return MethodTo(methodName, mce, ref type);
                }
            }
            else if (exp is ConstantExpression)
            {
                type = MemberType.Value;
                ConstantExpression ce = ((ConstantExpression)exp);
                if (ce.Value == null)
                    return "null";
                else if (ce.Value is Boolean)
                {
                    return Convert.ToBoolean(ce.Value) ? "1=1" : "1<>1";
                }
                else
                {
                    return ce.Value.ToString();
                }
            }
            else if (exp is MemberExpression)
            {
                MemberExpression me = ((MemberExpression)exp);
                if (me.Expression == null || me.Expression.NodeType.ToString() != "Parameter")
                {
                    type = MemberType.Value;
                    object dynInv = null;
                    try
                    {
                        // var dynInv = Expression.Lambda(exp).Compile().DynamicInvoke();原始写法性能极慢，下面写法性能提高了几十倍
                        // var dynInv= Expression.Lambda(me.Expression as ConstantExpression).Compile().DynamicInvoke();
                        var conExp = me.Expression as ConstantExpression;
                        if (conExp != null)
                        {
                            dynInv = (me.Member as System.Reflection.FieldInfo).GetValue((me.Expression as ConstantExpression).Value);
                        }
                        else
                        {

                            var memberInfos = new Stack<MemberInfo>();

                            // "descend" toward's the root object reference:
                            while (exp is MemberExpression)
                            {
                                var memberExpr = exp as MemberExpression;
                                memberInfos.Push(memberExpr.Member);
                                exp = memberExpr.Expression;
                            }

                            // fetch the root object reference:
                            var constExpr = exp as ConstantExpression;
                            var objReference = constExpr.Value;

                            // "ascend" back whence we came from and resolve object references along the way:
                            while (memberInfos.Count > 0)  // or some other break condition
                            {
                                var mi = memberInfos.Pop();
                                if (mi.MemberType == MemberTypes.Property)
                                {
                                    objReference = objReference.GetType()
                                                               .GetProperty(mi.Name)
                                                               .GetValue(objReference, null);
                                }
                                else if (mi.MemberType == MemberTypes.Field)
                                {
                                    objReference = objReference.GetType()
                                                               .GetField(mi.Name)
                                                               .GetValue(objReference);
                                }
                            }
                            dynInv = objReference;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (me.ToString() == "DateTime.Now")
                        {
                            return DateTime.Now.ToString();
                        }
                        Check.Exception(true, "错误信息:{0} \r\n message:{1}", "拉姆达解析出错", ex.Message);
                    }

                    if (dynInv == null) return null;
                    else
                        return dynInv.ToString();
                }
                else
                {
                    string name = me.Member.Name;
                    type = MemberType.Key;
                    return name;
                }
            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);
                var mex = ue.Operand;
                return CreateSqlElements(mex, ref type, false);
            }
            return null;
        }

        /// <summary>
        /// 获取操作符
        /// </summary>
        /// <param name="expressionType"></param>
        /// <returns></returns>
        private string GetOperator(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return " = ";
                case ExpressionType.GreaterThan:
                    return " > ";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                default:
                    throw new Exception(string.Format("不支持{0}此种运算符查找！" + expressionType));
            }
        }

        private string AddParas(ref string left, string right)
        {
            string oldLeft = left;
            left = left + SameIndex;
            SameIndex++;
            if (right == null)
            {
                this.Params.Add(new MySqlParameter("@" + left, DBNull.Value));
            }
            else
            {
                this.Params.Add(new MySqlParameter("@" + left, right));
            }
            return oldLeft;
        }

        private string Contains(string methodName, MethodCallExpression mce, bool isTure)
        {
            MemberType leftType = MemberType.None;
            MemberType rightType = MemberType.None;
            var left = CreateSqlElements(mce.Object, ref leftType);
            var right = CreateSqlElements(mce.Arguments[0], ref rightType);
            var oldLeft = AddParas(ref left, right);
            return string.Format("({0} {1} LIKE '%'+@{2}+'%')", oldLeft, isTure == false ? "  NOT " : null, left);
        }


        private string StartsWith(string methodName, MethodCallExpression mce, bool isTure)
        {
            MemberType leftType = MemberType.None;
            MemberType rightType = MemberType.None;
            var left = CreateSqlElements(mce.Object, ref leftType);
            var right = CreateSqlElements(mce.Arguments[0], ref rightType);
            var oldLeft = AddParas(ref left, right);
            return string.Format("({0} {1} LIKE @{2}+'%')", oldLeft, isTure == false ? "  NOT " : null, left);
        }
        private string EndWith(string methodName, MethodCallExpression mce, bool isTure)
        {
            MemberType leftType = MemberType.None;
            MemberType rightType = MemberType.None;
            var left = CreateSqlElements(mce.Object, ref leftType);
            var right = CreateSqlElements(mce.Arguments[0], ref rightType);
            var oldLeft = AddParas(ref left, right);
            return string.Format("({0} {1} LIKE '%'+@{2})", oldLeft, isTure == false ? "  NOT " : null, left);
        }

        private string MethodTo(string methodName, MethodCallExpression mce, ref MemberType type)
        {
            var value = CreateSqlElements(mce.Arguments[0], ref type);
            if (methodName == "ToDateTime")
            {
                return Convert.ToDateTime(value).ToString();
            }
            else if (methodName.StartsWith("ToInt"))
            {
                return Convert.ToInt32(value).ToString();
            }
            return value;
        }

        private string MethodToString(string methodName, MethodCallExpression mce, ref MemberType type)
        {
            return CreateSqlElements(mce.Object, ref type);
        }

    }
}
