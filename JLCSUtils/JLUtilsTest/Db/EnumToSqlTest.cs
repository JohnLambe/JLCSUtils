﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.Db;
using JohnLambe.Util;

namespace JohnLambe.Tests.JLUtilsTest.Db
{
    [TestClass]
    public class EnumToSqlTest
    {
        [TestMethod]
        public void GenerateSql()
        {
            EnumToSql enumToSql = new EnumToSql();
            string sql = enumToSql.ToSql(typeof(System.StringComparison));
            Console.WriteLine(sql);
            Console.WriteLine();

            Assert.AreEqual(("create table StringComparison (Id int not null primary key, Name varchar(50) );\n"
            + "insert into StringComparison (Id, Name) values( 0, \"CurrentCulture\" );\n"
            + "insert into StringComparison (Id, Name) values( 1, \"CurrentCultureIgnoreCase\" );\n"
            + "insert into StringComparison (Id, Name) values( 2, \"InvariantCulture\" );\n"
            + "insert into StringComparison (Id, Name) values( 3, \"InvariantCultureIgnoreCase\" );\n"
            + "insert into StringComparison (Id, Name) values( 4, \"Ordinal\" );\n"
            + "insert into StringComparison (Id, Name) values( 5, \"OrdinalIgnoreCase\" );\n")
            .RemoveCharacters(" \r\n"),
            sql.RemoveCharacters(" \r\n"));

        }
    }
}
