insert into dbo.DynamicType (Name) Values ('string');
insert into dbo.DynamicTemplate (Name, Description) Values ('SomeTable', 'Dynamic Table');
insert into dbo.DynamicAttribute (Name) Values ('Firstname');
insert into dbo.DynamicAttribute (Name) Values ('Surname');
insert into dbo.DynamicAttribute (Name) Values ('Gender');
insert into dbo.DynamicTemplateAttributes (TemplateId, AttributeId, DisplayName, TypeId, Idx)
	values ((select Id from dbo.DynamicTemplate where Name = 'SomeTable'),
			(select Id from dbo.DynamicAttribute where Name = 'Firstname'),
			'Firstname',
			(select Id from dbo.DynamicType where Name='string'),
			0);
insert into dbo.DynamicTemplateAttributes (TemplateId, AttributeId, DisplayName, TypeId, Idx)
	values ((select Id from dbo.DynamicTemplate where Name = 'SomeTable'),
			(select Id from dbo.DynamicAttribute where Name = 'Surname'),
			'Surname',
			(select Id from dbo.DynamicType where Name='string'),
			(select max(Idx) from dbo.DynamicTemplateAttributes) + 1);		
insert into dbo.DynamicTemplateAttributes (TemplateId, AttributeId, DisplayName, TypeId, Idx)
	values ((select Id from dbo.DynamicTemplate where Name = 'SomeTable'),
			(select Id from dbo.DynamicAttribute where Name = 'Gender'),
			'Gender',
			(select Id from dbo.DynamicType where Name='string'),
			(select max(Idx) from dbo.DynamicTemplateAttributes) + 1);	

insert into dbo.SomeTable (Firstname, Surname, Gender) Values ('Igor', 'Kors', 'Male');
insert into dbo.SomeTable (Firstname, Surname, Gender) Values ('Howzit', 'MyFriend', 'Unknown');
insert into dbo.SomeTable (Firstname, Surname, Gender) Values ('My', 'Mother', 'Female');