using System.Collections.Generic;
using NUnit.Framework;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite.Tests;

namespace NServiceKit.OrmLite.PostgreSQL.Tests
{
    /// <summary>An ORM lite execute procedure tests.</summary>
    [TestFixture]
    public class OrmLiteExecuteProcedureTests : OrmLiteTestBase
    {
        /// <summary>The create.</summary>
        private const string Create = @"
            CREATE OR REPLACE FUNCTION f_service_stack(
                v_string_values CHARACTER VARYING[],
                v_integer_values INTEGER[]
            ) RETURNS BOOLEAN AS
            $BODY$
            BEGIN
                IF v_string_values[1] <> 'NServiceKit' THEN
                    RAISE EXCEPTION 'Unexpected value in string array[1] %', v_string_values[1];
                END IF;
                IF v_string_values[2] <> 'Thoughtfully Architected' THEN
                    RAISE EXCEPTION 'Unexpected value in string array[2] %', v_string_values[2];
                END IF;
                IF v_integer_values[1] <> 1 THEN
                    RAISE EXCEPTION 'Unexpected value in integer array[1] %', v_integer_values[1];
                END IF;
                IF v_integer_values[2] <> 2 THEN
                    RAISE EXCEPTION 'Unexpected value in integer array[2] %', v_integer_values[2];
                END IF;
                IF v_integer_values[3] <> 3 THEN
                    RAISE EXCEPTION 'Unexpected value in integer array[3] %', v_integer_values[3];
                END IF;
                RETURN TRUE;
            END;
            $BODY$
            LANGUAGE plpgsql VOLATILE COST 100;
            ";

        /// <summary>The drop.</summary>
        private const string Drop = "DROP FUNCTION f_service_stack(CHARACTER VARYING[], INTEGER[]);";

        /// <summary>A service kit function.</summary>
        [Alias("f_service_stack")]
        public class NServiceKitFunction
        {
            /// <summary>Gets or sets the string values.</summary>
            /// <value>The string values.</value>
            public string[] StringValues { get; set; }

            /// <summary>Gets or sets the integer values.</summary>
            /// <value>The integer values.</value>
            public int[] IntegerValues { get; set; }
        }

        /// <summary>Can execute stored procedure with array arguments.</summary>
        [Test]
        public void Can_execute_stored_procedure_with_array_arguments()
        {
            using (var db = OpenDbConnection())
            {
                db.ExecuteSql(Create);

                db.ExecuteProcedure(new NServiceKitFunction
                                        {
                                            StringValues = new[] { "NServiceKit", "Thoughtfully Architected" },
                                            IntegerValues = new[] { 1, 2, 3 }
                                        });
                db.ExecuteSql(Drop);
            }
        }
    }
}
