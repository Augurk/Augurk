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

CREATE PROCEDURE [dbo].[InsertOrUpdateFeature]
	@title VARCHAR(255),
	@branchName VARCHAR(255),
	@groupName VARCHAR(255),
	@parentTitle VARCHAR(255),
	@serializedFeature TEXT
AS
BEGIN
	MERGE Feature 
	USING (SELECT @title, @branchName, @groupName, @parentTitle, @serializedFeature) AS newFeature (Title, BranchName, GroupName, ParentTitle, SerializedFeature)
	ON Feature.Title = newFeature.Title
	AND Feature.BranchName = newFeature.BranchName
	AND Feature.GroupName = newFeature.GroupName
	WHEN MATCHED THEN
		UPDATE SET SerializedFeature = newFeature.SerializedFeature, ParentTitle = newFeature.ParentTitle
	WHEN NOT MATCHED THEN
		INSERT(Title, BranchName, GroupName, ParentTitle, SerializedFeature)
		VALUES (newFeature.Title, newFeature.BranchName, newFeature.GroupName, newFeature.ParentTitle, newFeature.SerializedFeature);
END
