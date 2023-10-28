import {Pipe, PipeTransform} from '@angular/core';
import {format} from 'sql-formatter';
import {DatabaseType} from "../models/execution-trace";
import {SqlLanguage} from "sql-formatter/lib/src/sqlFormatter";
import {Dictionary} from "../models/dictionary";

@Pipe({
  name: 'sqlFormat',
  standalone: true
})
export class SqlFormatPipe implements PipeTransform {

  private readonly dialects: Dictionary<DatabaseType, SqlLanguage> = {
    sql: 'sql',
    inMemory: 'sqlite',
    sqlite: 'sqlite',
    sqlServer: 'transactsql',
    mySql: 'mysql',
  };

  transform(sql: string, databaseType?: DatabaseType): string {
    let dialect = databaseType !== undefined ? this.dialects[databaseType] : 'sql';
    return format(sql, {language: dialect, useTabs: false, indentStyle: 'tabularLeft', expressionWidth: 100});
  }

}
