
-- SCHEMAS
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.SCHEMATA where SCHEMA_NAME=N'cat')
	exec sp_executesql N'create schema cat';
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.SCHEMATA where SCHEMA_NAME=N'doc')
	exec sp_executesql N'create schema doc';
go
------------------------------------------------
begin
	set nocount on;
	grant execute on schema ::cat to public;
	grant execute on schema ::doc to public;
end
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.SEQUENCES where SEQUENCE_SCHEMA = N'cat' and SEQUENCE_NAME = N'SQ_Units')
	create sequence cat.SQ_Units as bigint start with 100 increment by 1;
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'cat' and TABLE_NAME=N'Units')
create table cat.Units
(
	Id bigint not null not null
		constraint DF_Units_Id default (next value for cat.SQ_Units),
	Void bit not null
		constraint DF_Units_Void default (0),
	[Name] nvarchar(255),
	Memo nvarchar(255),
	constraint PK_Units primary key (Id)
);
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.SEQUENCES where SEQUENCE_SCHEMA = N'cat' and SEQUENCE_NAME = N'SQ_Products')
	create sequence cat.SQ_Products as bigint start with 100 increment by 1;
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'cat' and TABLE_NAME=N'Products')
create table cat.Products
(
	Id bigint not null not null
		constraint DF_Products_Id default (next value for cat.SQ_Products),
	Void bit not null
		constraint DF_Products_Void default (0),
	[Name] nvarchar(255),
	SKU nvarchar(50),
	Unit bigint
		/* constraint FK_Products_Unit_Units foreign key references cat.Units(Id) */,
	Memo nvarchar(255),
	constraint PK_Products primary key (Id)
);
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.SEQUENCES where SEQUENCE_SCHEMA = N'cat' and SEQUENCE_NAME = N'SQ_Agents')
	create sequence cat.SQ_Agents as bigint start with 100 increment by 1;
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'cat' and TABLE_NAME=N'Agents')
create table cat.Agents
(
	Id bigint not null not null
		constraint DF_Agents_Id default (next value for cat.SQ_Agents),
	Void bit not null
		constraint DF_Agents_Void default (0),
	[Name] nvarchar(255),
	Memo nvarchar(255),
	Code nvarchar(50),
	constraint PK_Agents primary key (Id)
);
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.SEQUENCES where SEQUENCE_SCHEMA = N'doc' and SEQUENCE_NAME = N'SQ_Documents')
	create sequence doc.SQ_Documents as bigint start with 100 increment by 1;
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'doc' and TABLE_NAME=N'Documents')
create table doc.Documents
(
	Id bigint not null not null
		constraint DF_Documents_Id default (next value for doc.SQ_Documents),
	Void bit not null
		constraint DF_Documents_Void default (0),
	[Date] date,
	[No] nvarchar(50),
	Memo nvarchar(255),
	Agent bigint
		/* constraint FK_Documents_Agent_Agents foreign key references cat.Agents(Id) */,
	constraint PK_Documents primary key (Id)
);
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.SEQUENCES where SEQUENCE_SCHEMA = N'doc' and SEQUENCE_NAME = N'SQ_DocDetails')
	create sequence doc.SQ_DocDetails as bigint start with 100 increment by 1;
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'doc' and TABLE_NAME=N'DocDetails')
create table doc.DocDetails
(
	Id bigint not null not null
		constraint DF_DocDetails_Id default (next value for doc.SQ_DocDetails),
	Document bigint not null
		/* constraint FK_DocDetails_Document_Documents foreign key references doc.Documents(Id) */,
	Product bigint
		/* constraint FK_DocDetails_Product_Products foreign key references cat.Products(Id) */,
	Qty float,
	Price float,
	[Sum] money,
	constraint PK_DocDetails primary key (Id)
);
go
------------------------------------------------
-- Foreign keys
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.CONSTRAINT_TABLE_USAGE where TABLE_SCHEMA = N'cat' and TABLE_NAME = N'Products' and CONSTRAINT_NAME = N'FK_Products_Unit_Units')
	alter table cat.Products add
		constraint FK_Products_Unit_Units foreign key references cat.Units(Id);
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.CONSTRAINT_TABLE_USAGE where TABLE_SCHEMA = N'doc' and TABLE_NAME = N'Documents' and CONSTRAINT_NAME = N'FK_Documents_Agent_Agents')
	alter table doc.Documents add
		constraint FK_Documents_Agent_Agents foreign key references cat.Agents(Id);
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.CONSTRAINT_TABLE_USAGE where TABLE_SCHEMA = N'doc' and TABLE_NAME = N'DocDetails' and CONSTRAINT_NAME = N'FK_DocDetails_Document_Documents')
	alter table doc.DocDetails add
		constraint FK_DocDetails_Document_Documents foreign key references doc.Documents(Id);
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.CONSTRAINT_TABLE_USAGE where TABLE_SCHEMA = N'doc' and TABLE_NAME = N'DocDetails' and CONSTRAINT_NAME = N'FK_DocDetails_Product_Products')
	alter table doc.DocDetails add
		constraint FK_DocDetails_Product_Products foreign key references cat.Products(Id);
go
