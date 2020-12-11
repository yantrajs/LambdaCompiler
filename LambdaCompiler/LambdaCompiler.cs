using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;

namespace YantraJS.LambdaCompiler
{
    internal interface ILocalCache
    {
        LocalBuilder GetLocal(Type type);

        void FreeLocal(LocalBuilder local);
    }

    public partial class LambdaCompiler: ILocalCache
    {

        [Flags]
        internal enum CompilationFlags
        {
            EmitExpressionStart = 0x0001,
            EmitNoExpressionStart = 0x0002,
            EmitAsDefaultType = 0x0010,
            EmitAsVoidType = 0x0020,
            EmitAsTail = 0x0100,   // at the tail position of a lambda, tail call can be safely emitted
            EmitAsMiddle = 0x0200, // in the middle of a lambda, tail call can be emitted if it is in a return
            EmitAsNoTail = 0x0400, // neither at the tail or in a return, or tail call is not turned on, no tail call is emitted

            EmitExpressionStartMask = 0x000f,
            EmitAsTypeMask = 0x00f0,
            EmitAsTailCallMask = 0x0f00
        }

        private readonly Dictionary<ParameterExpression, LocalBuilder> variables
            = new Dictionary<ParameterExpression, LocalBuilder>();
        private readonly Dictionary<ParameterExpression, int> arguments;

        private readonly List<LocalBuilder> tempVariables = new List<LocalBuilder>();
        private readonly List<LocalBuilder> freeVariables = new List<LocalBuilder>();
        private readonly ILGenerator il;
        private readonly ILGenerator _ilg;
        private readonly LambdaGenerator generator;

        public LambdaCompiler(ILGenerator il, LambdaGenerator generator)
        {
            this.il = il;
            this._ilg = il;
            this.generator = generator;
        }

        //private void EmitExpression(Expression body)
        //{
        //    switch ((body.NodeType, body))
        //    {
        //        case (ExpressionType.Parameter, ParameterExpression pe):
        //            EmitParameter(pe);
        //            break;
        //        case (ExpressionType.Block, BlockExpression be):
        //            Emit(be);
        //            break;
        //        case (ExpressionType.TypeAs, UnaryExpression typeAsExpression):
        //            EmitTypeAs(typeAsExpression);
        //            break;
        //        case (ExpressionType.IsTrue, UnaryExpression isTrueExpression):
        //            EmitIsTrue(isTrueExpression);
        //            break;
        //        case (ExpressionType.IsFalse, UnaryExpression isFalseExpression):
        //            EmitIsFalse(isFalseExpression);
        //            break;
        //        case (ExpressionType.Increment, UnaryExpression incrementExpression):
        //            EmitIncrement(incrementExpression);
        //            break;
        //        case (ExpressionType.Decrement, UnaryExpression decrementExpression):
        //            EmitDecrement(decrementExpression);
        //            break;
        //        case (ExpressionType.Negate, UnaryExpression negateExpression):
        //            EmitNegate(negateExpression);
        //            break;
        //        case (ExpressionType.NegateChecked, UnaryExpression negateCheckedExpression):
        //            EmitNegate(negateCheckedExpression);
        //            break;
        //        case (ExpressionType.OnesComplement, UnaryExpression onesComplementExpression):
        //            EmitOnesComplement(onesComplementExpression);
        //            break;
        //        case (ExpressionType.UnaryPlus, UnaryExpression unaryPlusExpression):
        //            EmitUnaryPlus(unaryPlusExpression);
        //            break;
        //        case (ExpressionType.Unbox, UnaryExpression unboxExpression):
        //            throw new NotSupportedException();
        //        case (ExpressionType.TypeIs, TypeBinaryExpression typeIsExpression):
        //            EmitTypeIsExpression(typeIsExpression);
        //            break;
        //        case (ExpressionType.TypeEqual, TypeBinaryExpression typeEqualExpression):
        //            EmitTypeIsExpression(typeEqualExpression);
        //            break;
        //        case (ExpressionType.Not, UnaryExpression notExpression):
        //            EmitNotExpression(notExpression);
        //            break;

        //    }
        //}


        public LocalBuilder GetLocal(Type type)
        {
            var v = freeVariables.FirstOrDefault(x => x.LocalType == type);
            if (v == null)
            {
                v = il.DeclareLocal(type);
                tempVariables.Add(v);
            }
            return v;
        }

        public void FreeLocal(LocalBuilder local)
        {
            freeVariables.Add(local);
        }
    }
}
