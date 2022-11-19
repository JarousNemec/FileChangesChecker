// See https://aka.ms/new-console-template for more information

using FileChangesChecker;

FileChecker checker = new FileChecker();
checker.Check("./checks/check.txt", "./dirToCheck/");