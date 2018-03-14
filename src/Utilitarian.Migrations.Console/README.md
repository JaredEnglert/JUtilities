Migration Console Application
=============================

Migration Commands
------------------

migrate-up-pre-release -dt=MongoDB -dn=FirstTest -t=FirstTopic

migrate-up-post-release -dt=MongoDB -dn=FirstTest -t=FirstTopic

migrate-down-pre-rollback -dt=MongoDB -dn=FirstTest -t=FirstTopic

migrate-down-post-rollback -dt=MongoDB -dn=FirstTest -t=FirstTopic

Database Commands
-----------------

drop-database -dt=MongoDB -dn=FirstTest
