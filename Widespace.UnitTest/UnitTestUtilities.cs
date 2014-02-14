using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Widespace.UnitTest
{
    public class UnitTestUtilities
    {
        private UnitTestUtilities()
        {

        }

        #region Run Method

        /// <summary>
        ///		Runs a method on a type, given its parameters. This is useful for
        ///		calling private methods.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="strMethod"></param>
        /// <param name="aobjParams"></param>
        /// <returns>The return value of the called method.</returns>
        public static object RunStaticMethod(System.Type t, string strMethod, object[] aobjParams)
        {
            BindingFlags eFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            return RunMethod(t, strMethod, null, aobjParams, eFlags);
        } //end of method

        public static object RunInstanceMethod(System.Type t, string strMethod, object objInstance, object[] aobjParams)
        {
            BindingFlags eFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            return RunMethod(t, strMethod, objInstance, aobjParams, eFlags);
        } //end of method

        private static object RunMethod(System.Type t, string strMethod, object objInstance, object[] aobjParams, BindingFlags eFlags)
        {
            MethodInfo m;
            try
            {
                m = t.GetMethod(strMethod, eFlags);
                if (m == null)
                {
                    throw new ArgumentException("There is no method '" + strMethod + "' for type '" + t.ToString() + "'.");
                }

                object objRet = m.Invoke(objInstance, aobjParams);
                return objRet;
            }
            catch(Exception e)
            {
                throw e.InnerException;
            }
        } //end of method

        #endregion
    }
}
