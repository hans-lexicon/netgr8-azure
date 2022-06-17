CREATE TABLE Measurements (
	Id uniqueidentifier not null primary key,
	DeviceId nvarchar(max) not null,
	MeasurementTime datetime2 not null,
	Temperature float not null,
	Humidity float not null,
	AlertNotification bit not null
)