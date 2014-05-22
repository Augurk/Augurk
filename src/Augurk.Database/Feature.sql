/*
 Copyright 2014, Mark Taling
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
 http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
*/

CREATE TABLE [dbo].[Feature]
(
	[Id] INT NOT NULL IDENTITY, 
    [Title] NVARCHAR(255) NOT NULL, 
    [BranchName] NVARCHAR(255) NOT NULL, 
	[GroupName] NVARCHAR(255) NOT NULL,
	[ParentTitle] NVARCHAR(255) NULL,
    [SerializedFeature] TEXT NOT NULL,

    CONSTRAINT [PK_Feature] PRIMARY KEY NONCLUSTERED ([Id] ASC)
)

GO

CREATE UNIQUE CLUSTERED INDEX [IXU001_Feature] ON [dbo].[Feature] ([BranchName] ASC, [GroupName] ASC, [Title] ASC)
