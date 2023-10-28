export type DatabaseType = 'sql' | 'inMemory' | 'sqlite' | 'sqlServer' | 'mySql';

export interface ExecutionTrace {
  sourcecode: string;
  databaseTraces: DatabaseTrace[];
}

export interface DatabaseTrace {
  databaseType: DatabaseType;
  sqlCommands: string[];
  sqlCommandsFormatted: string;
  result: any;
  resultJson: string;
}
