using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnLambe.Tests.JLUtilsTest
{
    [TestClass]
    public class LazyInitializeTest
    {
        private LazyInitialzed<List<int>> _lazyInit = new LazyInitialzed<List<int>>( () => new List<int>( new int[] { 2, 4, 6 } ) );

        [TestMethod]
        public void LazyInitialize1Test()
        {
            List<int> x = _lazyInit;
            x.First();
            Assert.AreEqual( 2, ( (List<int>) _lazyInit).First() );
//            foreach(var y in _lazyInit)
            {

            }
        }


        private LazyInitialzed<List<int>> _lazyInit1B =
            new LazyInitialzed<List<int>>(() => new List<int>(new int[] { 2, 4, 6 }));
        public List<int> LazyInit1B { get { return _lazyInit1B; } }

        [TestMethod]
        public void LazyInitialize1BTest()
        {
            Assert.AreEqual(2, LazyInit1B.First());
        }


        private List<int> _lazyInit2Field;
        public List<int> LazyInit2 { 
            get { return LazyInitialize.GetValue(ref _lazyInit2Field,
                () => new List<int>(new int[] { 2, 4, 6 })); }
        }

//        private LazyInitialze<List<int>> _lazyInit1b = new LazyInitialze<List<int>>(() => new List<int>(new int[] { 2, 4, 6 })); public List<int> LazyInit1B { get { return _lazyInit1b; } }
//        private List<int> _lazyInit2a; public List<int> LazyInit2 { get { return LazyInitialize2.GetValue(ref _lazyInit2a, () => new List<int>(new int[] { 2, 4, 6 })); } }
    
    }

}
