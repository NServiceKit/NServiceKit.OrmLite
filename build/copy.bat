REM SET BUILD=Debug
SET BUILD=Release
SET MSBUILD=C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe 

MD ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite32\lib\
MD ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite32\lib\net35
MD ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite32\lib\net40
MD ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite64\lib\net35
MD ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite64\lib\net40
MD ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.SqlServer\lib
MD ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite.Mono\lib
MD ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite.Mono\lib\net35
MD ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite.Mono\lib\net40
MD ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.MySql\lib
MD ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.PostgreSQL\lib
MD ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Oracle\lib
MD ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Firebird\lib
MD ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.T4\content

COPY ..\src\NServiceKit.OrmLite.Sqlite32\bin\%BUILD%\NServiceKit.OrmLite.*  ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite32\lib\net35
COPY ..\src\NServiceKit.OrmLite.Sqlite32\bin\x86\NServiceKit.OrmLite.*  ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite32\lib\net40
COPY ..\src\NServiceKit.OrmLite.Sqlite64\bin\%BUILD%\NServiceKit.OrmLite.*  ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite64\lib\net35
COPY ..\src\NServiceKit.OrmLite.Sqlite64\bin\x64\NServiceKit.OrmLite.*  ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite64\lib\net40
COPY ..\src\NServiceKit.OrmLite.SqlServer\bin\%BUILD%\NServiceKit.OrmLite.* ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.SqlServer\lib

COPY ..\lib\Mono.Data.Sqlite.dll  ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite.Mono\lib\net35
COPY ..\src\NServiceKit.OrmLite.Sqlite\bin\%BUILD%\NServiceKit.OrmLite.*  ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite.Mono\lib\net35
COPY ..\lib\Mono.Data.Sqlite.dll  ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite.Mono\lib\net40
COPY ..\src\NServiceKit.OrmLite.Sqlite\bin\x86\NServiceKit.OrmLite.*  ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Sqlite.Mono\lib\net40

COPY ..\src\T4\*.*  ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.T4\content


COPY ..\src\NServiceKit.OrmLite.MySql\bin\%BUILD%\NServiceKit.OrmLite.* ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.MySql\lib
COPY ..\src\NServiceKit.OrmLite.PostgreSQL\bin\%BUILD%\NServiceKit.OrmLite.* ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.PostgreSQL\lib
COPY ..\src\NServiceKit.OrmLite.Oracle\bin\%BUILD%\NServiceKit.OrmLite.* ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Oracle\lib
COPY ..\src\NServiceKit.OrmLite.Firebird\bin\%BUILD%\NServiceKit.OrmLite.* ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Firebird\lib
COPY ..\src\NServiceKit.OrmLite.Firebird\bin\%BUILD%\FirebirdSql.Data.FirebirdClient.dll ..\..\NServiceKit.OrmLite\NuGet\NServiceKit.OrmLite.Firebird\lib

COPY ..\src\NServiceKit.OrmLite.SqlServer\bin\%BUILD%\*.* ..\..\NServiceKit\release\latest\NServiceKit.OrmLite
COPY ..\src\NServiceKit.OrmLite.Sqlite32\bin\%BUILD%\NServiceKit.OrmLite.SqliteNET.*  ..\..\NServiceKit\release\latest\NServiceKit.OrmLite\x32
COPY ..\src\NServiceKit.OrmLite.Sqlite64\bin\%BUILD%\NServiceKit.OrmLite.SqliteNET.*  ..\..\NServiceKit\release\latest\NServiceKit.OrmLite\x64
COPY ..\lib\x32\net35\System.Data.SQLite.dll  ..\..\NServiceKit\release\latest\NServiceKit.OrmLite\x32\net35
COPY ..\lib\x32\net40\System.Data.SQLite.dll  ..\..\NServiceKit\release\latest\NServiceKit.OrmLite\x32\net40
COPY ..\lib\x64\net35\System.Data.SQLite.dll  ..\..\NServiceKit\release\latest\NServiceKit.OrmLite\x64\net35
COPY ..\lib\x64\net40\System.Data.SQLite.dll  ..\..\NServiceKit\release\latest\NServiceKit.OrmLite\x64\net40

COPY ..\src\NServiceKit.OrmLite.Sqlite\bin\x86\NServiceKit.OrmLite.* ..\..\NServiceKit\lib
COPY ..\src\NServiceKit.OrmLite.Sqlite\bin\x86\sqlite3.dll ..\..\NServiceKit\lib
COPY ..\src\NServiceKit.OrmLite.Sqlite\bin\x86\Mono.Data.Sqlite.dll ..\..\NServiceKit\lib
COPY ..\src\NServiceKit.OrmLite.Sqlite32\bin\x86\NServiceKit.OrmLite.Sqlite* ..\..\NServiceKit\lib\x86
COPY ..\src\NServiceKit.OrmLite.Sqlite64\bin\x64\NServiceKit.OrmLite.Sqlite* ..\..\NServiceKit\lib\x64
COPY ..\src\NServiceKit.OrmLite.SqlServer\bin\%BUILD%\NServiceKit.OrmLite.* ..\..\NServiceKit\lib

COPY ..\src\NServiceKit.OrmLite.Sqlite\bin\%BUILD%\NServiceKit.OrmLite.* ..\..\NServiceKit.Examples\lib
COPY ..\src\NServiceKit.OrmLite.Sqlite\bin\%BUILD%\NServiceKit.OrmLite.* ..\..\NServiceKit.Contrib\lib
