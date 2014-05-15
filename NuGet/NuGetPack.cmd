SET NUGET=..\src\.nuget\nuget
%NUGET% pack NServiceKit.OrmLite.Sqlite.Mono\NServiceKit.ormlite.sqlite.mono.nuspec -symbols
%NUGET% pack NServiceKit.OrmLite.Sqlite32\NServiceKit.ormlite.sqlite32.nuspec -symbols
%NUGET% pack NServiceKit.OrmLite.Sqlite64\NServiceKit.ormlite.sqlite64.nuspec -symbols
%NUGET% pack NServiceKit.OrmLite.SqlServer\NServiceKit.ormlite.sqlserver.nuspec -symbols
%NUGET% pack NServiceKit.OrmLite.MySql\NServiceKit.ormlite.mysql.nuspec -symbols
%NUGET% pack NServiceKit.OrmLite.PostgreSQL\NServiceKit.ormlite.postgresql.nuspec -symbols
%NUGET% pack NServiceKit.OrmLite.Oracle\NServiceKit.ormlite.oracle.nuspec -symbols
%NUGET% pack NServiceKit.OrmLite.Firebird\NServiceKit.ormlite.firebird.nuspec -symbols
%NUGET% pack NServiceKit.OrmLite.T4\NServiceKit.ormlite.t4.nuspec -symbols

