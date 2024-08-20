using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Rockaway.WebApp.Data;

namespace Rockaway.WebApp.Tests;

class TestDatabase {
	public static RockawayDbContext Create(string? dbName = null) {
		dbName ??= Guid.NewGuid().ToString();
		var dbContext = Connect(dbName);
		dbContext.Database.EnsureCreated();
		return dbContext;
	}

	public static RockawayDbContext Connect(string dbName) {
		var connectionString = $"Data Source={dbName};Mode=Memory;Cache=Shared";
		var sqliteConnection = new SqliteConnection(connectionString);
		sqliteConnection.Open();
		var cmd = new SqliteCommand("PRAGMA case_sensitive_like = false", sqliteConnection);
		cmd.ExecuteNonQuery();
		var options = new DbContextOptionsBuilder<RockawayDbContext>()
			.UseSqlite(sqliteConnection)
			.Options;
		return new(options);
	}
}
