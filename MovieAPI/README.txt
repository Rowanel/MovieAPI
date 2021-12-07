Steps to get API up and running:

1. The API uses a Nuget package that helps with CSV. Ensure you have the Nuget Package installed for CSVHelper (latest stable version) before publishing.
2. The data files (metadata and stats) are located in the solution \\Data\\csvdata and it will create the files on debug execution only if the files are newer in the solution as opposed to files already in the bin folder. If the data files are missing just ensure they are in the location of the published/debugged build in \Data\csvdata
3. The calls are routed as per spec: /metadata/
