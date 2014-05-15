using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Data;

using NServiceKit.DataAnnotations;
using System.Reflection;

using NServiceKit.OrmLite;
using NServiceKit.OrmLite.Firebird;


namespace TestLiteFirebirdProcedures
{
    /// <summary>An employee.</summary>
    [Alias("EMPLOYEE")]
    public class Employee
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [Alias("EMP_NO")]
        [Sequence("EMP_NO_GEN")]
        public Int16 Id
        {
            get;
            set;
        }

        /// <summary>Gets or sets the person's first name.</summary>
        /// <value>The name of the first.</value>
        [Alias("FIRST_NAME")]
        [Required]
        public string FirstName
        {
            get;
            set;
        }

        /// <summary>Gets or sets the person's last name.</summary>
        /// <value>The name of the last.</value>
        [Alias("LAST_NAME")]
        [Required]
        public string LastName
        {
            get;
            set;
        }

        /// <summary>Gets or sets the phone extension.</summary>
        /// <value>The phone extension.</value>
        [Alias("PHONE_EXT")]
        public string PhoneExtension
        {
            get;
            set;
        }

        /// <summary>Gets or sets the hire date.</summary>
        /// <value>The hire date.</value>
        [Alias("HIRE_DATE")]
        [Required]
        public DateTime HireDate
        {
            get;
            set;
        }

        /// <summary>Gets or sets the departament number.</summary>
        /// <value>The departament number.</value>
        [Alias("DEPT_NO")]
        [Required]
        public string DepartamentNumber
        {
            get;
            set;
        }

        /// <summary>Gets or sets the job code.</summary>
        /// <value>The job code.</value>
        [Alias("JOB_CODE")]
        [Required]
        public string JobCode
        {
            get;
            set;
        }

        /// <summary>Gets or sets the job grade.</summary>
        /// <value>The job grade.</value>
        [Alias("JOB_GRADE")]
        public Int16 JobGrade
        {
            get;
            set;
        }

        /// <summary>Gets or sets the job country.</summary>
        /// <value>The job country.</value>
        [Alias("JOB_COUNTRY")]
        [Required]
        public string JobCountry
        {
            get;
            set;
        }

        /// <summary>Gets or sets the salary.</summary>
        /// <value>The salary.</value>
        [Alias("SALARY")]
        [Required]
        public Decimal Salary
        {
            get;
            set;
        }

    }

    /// <summary>A procedure delete employee.</summary>
    [Alias("DELETE_EMPLOYEE")]
    public class ProcedureDeleteEmployee
    {
        /// <summary>Gets or sets the employee number.</summary>
        /// <value>The employee number.</value>
        [Alias("EMP_NUM")]
        public Int16 EmployeeNumber
        {
            get;
            set;
        }

    }

    /// <summary>A procedure sub total budget parameters.</summary>
    [Alias("SUB_TOT_BUDGET")]
    public class ProcedureSubTotalBudgetParameters
    {
        /// <summary>Gets or sets the head departament.</summary>
        /// <value>The head departament.</value>
        [Alias("HEAD_DEPT")]
        public string HeadDepartament
        {
            get;
            set;
        }

    }

    /// <summary>Encapsulates the result of a procedure sub total budget.</summary>
    public class ProcedureSubTotalBudgetResult
    {
        /// <summary>Gets or sets the number of. </summary>
        /// <value>The total.</value>
        [Alias("TOT_BUDGET")]
        public decimal Total
        {
            get;
            set;
        }

        /// <summary>Gets or sets the average.</summary>
        /// <value>The average value.</value>
        [Alias("AVG_BUDGET")]
        public decimal Average
        {
            get;
            set;
        }

        /// <summary>Gets or sets the maximum.</summary>
        /// <value>The maximum value.</value>
        [Alias("MAX_BUDGET")]
        public decimal Max
        {
            get;
            set;
        }

        /// <summary>Gets or sets the minimum.</summary>
        /// <value>The minimum value.</value>
        [Alias("MIN_BUDGET")]
        public decimal Min
        {
            get;
            set;
        }
    }

    /// <summary>A procedure show langs parameters.</summary>
    [Alias("SHOW_LANGS")]
    public class ProcedureShowLangsParameters
    {
        /// <summary>Gets or sets the code.</summary>
        /// <value>The code.</value>
        [Alias("CODE")]
        public string Code
        {
            get;
            set;
        }

        /// <summary>Gets or sets the grade.</summary>
        /// <value>The grade.</value>
        [Alias("GRADE")]
        public Int16 Grade
        {
            get;
            set;
        }

        /// <summary>Gets or sets the country.</summary>
        /// <value>The country.</value>
        [Alias("CTY")]
        public string Country
        {
            get;
            set;
        }

    }

    /// <summary>Encapsulates the result of a procedure show langs.</summary>
    public class ProcedureShowLangsResult
    {
        /// <summary>Gets or sets the language.</summary>
        /// <value>The language.</value>
        [Alias("LANGUAGES")]
        public string Language
        {
            get;
            set;
        }
    }

    /// <summary>A procedure all langs.</summary>
    [Alias("ALL_LANGS")]
    public class ProcedureAllLangs
    {
        /// <summary>Executes the given database.</summary>
        /// <param name="db">The database.</param>
        /// <returns>A List&lt;ProcedureAllLangsResult&gt;</returns>
        public List<ProcedureAllLangsResult> Execute(IDbConnection db)
        {
            return db.SelectFromProcedure<ProcedureAllLangsResult>(this);
        }

        //public List<ProcedureAllLangsResult> Results{
        //	get; set;
        //}

        /// <summary>Encapsulates the result of a procedure all langs.</summary>
        public class ProcedureAllLangsResult
        {
            /// <summary>Gets or sets the code.</summary>
            /// <value>The code.</value>
            [Alias("CODE")]
            public string Code
            {
                get;
                set;
            }

            /// <summary>Gets or sets the grade.</summary>
            /// <value>The grade.</value>
            [Alias("GRADE")]
            public string Grade
            {
                get;
                set;
            }

            /// <summary>Gets or sets the country.</summary>
            /// <value>The country.</value>
            [Alias("COUNTRY")]
            public string Country
            {
                get;
                set;
            }

            /// <summary>Gets or sets the language.</summary>
            /// <value>The language.</value>
            [Alias("LANG")]
            public string Language
            {
                get;
                set;
            }

        }

    }

    /// <summary>A main class.</summary>
    class MainClass
    {
        /// <summary>Main entry-point for this application.</summary>
        /// <param name="args">Array of command-line argument strings.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            OrmLiteConfig.DialectProvider = new FirebirdOrmLiteDialectProvider();
            using (IDbConnection db =
			       "User=SYSDBA;Password=masterkey;Database=employee.fdb;DataSource=localhost;Dialect=3;charset=ISO8859_1;".OpenDbConnection())
            {
                try
                {

                    var employees = db.Select<Employee>();
                    Console.WriteLine("Total Employees '{0}'", employees.Count);

                    Employee employee = new Employee() {
                        FirstName = "LILA",
                        LastName = "FUTURAMA",
                        PhoneExtension = "0002",
                        HireDate = DateTime.Now,
                        DepartamentNumber = "900",
                        JobCode = "Eng",
                        JobGrade = 2,
                        JobCountry = "USA",
                        Salary = 75000
                    };
                    int count = employees.Count;

                    db.Insert(employee);
                    Console.WriteLine("Id for new employee : '{0}'", employee.Id);

                    employees = db.Select<Employee>();
                    Console.WriteLine("Total Employees '{0}' = '{1}'", employees.Count, count + 1);

                    Console.WriteLine("Executing 'DELETE_EMPLOYEE' for  '{0}' - {1}", employee.Id, employee.LastName);
                    ProcedureDeleteEmployee de = new ProcedureDeleteEmployee();
                    de.EmployeeNumber = employee.Id;
                    db.ExecuteProcedure(de);

                    employees = db.Select<Employee>();
                    Console.WriteLine("Total Employees '{0}'= '{1}' ", employees.Count, count);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }


                try
                {

                    ProcedureSubTotalBudgetParameters p = new ProcedureSubTotalBudgetParameters() {
                        HeadDepartament = "000"
                    };

                    var results = db.SelectFromProcedure<ProcedureSubTotalBudgetResult>(p, "");


                    foreach (var r in results)
                    {
                        Console.WriteLine("r.Total:{0} r.Average:{1} r.Max:{2} r.Min:{3}", r.Total, r.Average, r.Max, r.Min);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {

                    ProcedureShowLangsParameters l = new ProcedureShowLangsParameters() {
                        Code = "Sales",
                        Grade = 3,
                        Country = "England"
                    };

                    var ls = db.SelectFromProcedure<ProcedureShowLangsResult>(l, "");

                    foreach (var lr in ls)
                    {
                        Console.WriteLine(lr.Language);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {

                    ProcedureAllLangs l = new ProcedureAllLangs();

                    //var ls = db.SelectFromProcedure<ProcedureAllLangsResult>(l);
                    //db.SelectFromProcedure(l);


                    var ls = l.Execute(db);  // better ?

                    foreach (var lr in ls)
                    {
                        Console.WriteLine("lr.Code:{0} lr.Country:{1} lr.Grade:{2}  lr.Language:{3}",
                                          lr.Code, lr.Country, lr.Grade, lr.Language);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }


                Console.WriteLine("This is The End my friend!");
            }

        }
    }
}

