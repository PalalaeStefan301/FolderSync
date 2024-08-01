# FolderSync

- Mainly creates an object of type folder that manage all the differences between it and the actual folder that's been selected to be tracked, afterwards makes the updates on the replica folder.

- I've used builder design pattern to be able to add different features afterwards if needed, like even checking for renamed folder/files, or limit the tracking layer level. Like if you want to track only the first level of the folder or multiples, or adding a list of folder/files to be avoided.
