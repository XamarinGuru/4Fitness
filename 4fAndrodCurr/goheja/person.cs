using System;
using System.Data;
using Mono.Data.Sqlite;
using SQLite;
using System.Threading;
using System.Threading.Tasks;

namespace goheja
{
	public class Person
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public override string ToString()
		{
			return string.Format("[Person: ID={0}, FirstName={1}, LastName={2}]", ID, FirstName, LastName);
		}

		public string createDatabase(string path)
		{
			try
			{
				var connection = new SQLiteAsyncConnection(path);{
					connection.CreateTableAsync<Person>();
					return "Database created";
				}
			}
				catch (SQLiteException ex)
				{
					return ex.Message;
				}
		}

		public string insertUpdateData(Person data, string path)
		{
			try
			{
				var db = new SQLiteAsyncConnection(path);
				if ( db.InsertAsync(data) != Task.FromResult(0))
					db.UpdateAsync(data);
				return "Single data file inserted or updated";
			}
			catch (SQLiteException ex)
			{
				return ex.Message;
			}
		}
		public Task<int> findNumberRecords(string path)
		{
			try
			{
				var db = new SQLiteAsyncConnection(path);
				// this counts all records in the database, it can be slow depending on the size of the database
				var count = db.ExecuteScalarAsync<int>("SELECT Count(*) FROM Person");

				// for a non-parameterless query
				// var count = db.ExecuteScalar<int>("SELECT Count(*) FROM Person WHERE FirstName="Amy");

				return count;
			}
			catch (SQLiteException ex)
			{
				return Task.FromResult( -1);
			}
		}



	}


}

