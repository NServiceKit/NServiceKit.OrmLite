using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NServiceKit.OrmLite
{
    /// <summary>An ORM lite sp statement.</summary>
    public class OrmLiteSPStatement
    {
        /// <summary>Gets or sets the command.</summary>
        /// <value>The command.</value>
        private IDbCommand command { get; set; }

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.OrmLiteSPStatement class.
        /// </summary>
        /// <param name="cmd">The command.</param>
        public OrmLiteSPStatement(IDbCommand cmd)
        {
            command = cmd;
        }

        /// <summary>Converts this object to a list.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>object converted to a list.</returns>
        public List<T> ConvertToList<T>() where T : new()
        {
            if ((typeof(T).IsPrimitive) || (typeof(T) == typeof(string)) || (typeof(T) == typeof(String)))
                throw new Exception("Type " + typeof(T).Name + " is a primitive type. Use ConvertScalarToList function.");

            IDataReader reader = null;
            try
            {
                reader = command.ExecuteReader();
                return reader.ConvertToList<T>();
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>Converts this object to a scalar list.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>object converted to a scalar list.</returns>
        public List<T> ConvertToScalarList<T>()
        {
            if (!((typeof(T).IsPrimitive) || (typeof(T) == typeof(string)) || (typeof(T) == typeof(String))))
                throw new Exception("Type " + typeof(T).Name + " is a non primitive type. Use ConvertToList function.");

            IDataReader reader = null;
            try
            {
                reader = command.ExecuteReader();
#pragma warning disable 618
                return reader.GetFirstColumn<T>();
#pragma warning restore 618
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>Convert to.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>object converted to to.</returns>
        public T ConvertTo<T>() where T : new()
        {
            if ((typeof(T).IsPrimitive) || (typeof(T) == typeof(string)) || (typeof(T) == typeof(String)))
                throw new Exception("Type " + typeof(T).Name + " is a primitive type. Use ConvertScalarTo function.");

            IDataReader reader = null;
            try
            {
                reader = command.ExecuteReader();
                return reader.ConvertTo<T>();
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>Converts this object to a scalar.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>object converted to a scalar.</returns>
        public T ConvertToScalar<T>()
        {
            if (!((typeof(T).IsPrimitive) || (typeof(T) == typeof(string)) || (typeof(T) == typeof(String))))
                throw new Exception("Type " + typeof(T).Name + " is a non primitive type. Use ConvertTo function.");

            IDataReader reader = null;
            try
            {
                reader = command.ExecuteReader();
#pragma warning disable 618
                return reader.GetScalar<T>();
#pragma warning restore 618
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>Convert first column to list.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>object converted to a first column to list.</returns>
        public List<T> ConvertFirstColumnToList<T>()
        {
            if (!((typeof(T).IsPrimitive) || (typeof(T) == typeof(string)) || (typeof(T) == typeof(String))))
                throw new Exception("Type " + typeof(T).Name + " is a non primitive type. Only primitive type can be used.");

            IDataReader reader = null;
            try
            {
                reader = command.ExecuteReader();
#pragma warning disable 618
                return reader.GetFirstColumn<T>();
#pragma warning restore 618
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>Convert first column to list distinct.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>object converted to a first column to list distinct.</returns>
        public HashSet<T> ConvertFirstColumnToListDistinct<T>() where T : new()
        {
            if (!((typeof(T).IsPrimitive) || (typeof(T) == typeof(string)) || (typeof(T) == typeof(String))))
                throw new Exception("Type " + typeof(T).Name + " is a non primitive type. Only primitive type can be used.");

            IDataReader reader = null;
            try
            {
                reader = command.ExecuteReader();
#pragma warning disable 618
                return reader.GetFirstColumnDistinct<T>();
#pragma warning restore 618
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>Executes the non query operation.</summary>
        /// <returns>An int.</returns>
        public int ExecuteNonQuery()
        {
            return command.ExecuteNonQuery();
        }

        /// <summary>Query if this object has result.</summary>
        /// <returns>true if result, false if not.</returns>
        public bool HasResult()
        {
            IDataReader reader = null;
            try
            {
                reader = command.ExecuteReader();
                if (reader.Read())
                    return true;
                else
                    return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
