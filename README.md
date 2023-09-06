PRACTICAL TASK FOR .NET DEVELOPERS

Please develop a backend service using .Net core 6. The service should meet the
following requirements:

o Every 1 minute service connects to sftp and checks if there are new files.
o Sftp server, file paths etc. must be configurable (not in code).
o Service downloads all the new files to local path.
o All downloaded files (paths) should be stored in database (postgresql).
o Files from sftp are never deleted, so checking if file is new or old has to be done
by checking it in database taken in consideration file creation time.
o Work with database should by done by Entity framework
o Database should be defined by code first principle.
o Service should be resilient: handle connection problems etc. and should not
"die".
o Code must have comments explaining what it does.
o Service should have sane logging, configurable tracing (it should be clear what is
happening from logs).
o Service should use dependency injection.
