CREATE TABLE Device (
	Id nvarchar(256) not null primary key,
	SensorType nvarchar(max) not null,
	SharedAccessKey nvarchar(max) not null
)