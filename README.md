# Solution
To run the solution you should have an instance of a MySQL Database running and add
the following configuration to your __appsettings.json__ file.
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=your-server;User ID=your-user;Password=your-password;Database=your-database"
}
```

Also you need to add you JWT key, issuer and audience to the __appsettings.json__ file.
```json
"Jwt": {
    "Key": "your-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience"
}
```

After you have setup you __appsettings.json__ file you should be able to run the solution
without any problem executing the following command:
```bash
dotnet run
```

The first time you run the solution your database will seeded with the following categories:
- Groceries
- Leisure
- Electronics
- Utilities
- Clothing
- Health
- Others

and also with the following user:

- Username: test
- FirstName: Test
- LastName: User
- Password: pass1234

You can check all the endpoints and schemas used in each endpoint in the OpenAPI generated
documentation which is in the **/swagger** endpoint.

## Project URL
(Expense Tracker API | Roadmap)[https://roadmap.sh/projects/expense-tracker-api]