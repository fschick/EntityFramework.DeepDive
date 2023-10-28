import {AfterViewInit, Component, Input, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {NgbCollapseModule, NgbNav, NgbNavItem, NgbNavModule, NgbNavOutlet} from "@ng-bootstrap/ng-bootstrap";
import {BehaviorSubject, catchError, map, Observable, of, switchMap} from "rxjs";
import {ApiService} from "../../services/api.service";
import {ExecutionTrace} from "../../models/execution-trace";
import {HighlightJS, HighlightModule} from "ngx-highlightjs";
import {NgxJsonViewerModule} from "ngx-json-viewer";
import {CapitalizePipe} from "../../services/capitalize.pipe";
import {ClipboardModule} from "ngx-clipboard";
import {SqlFormatPipe} from "../../services/sql-format.pipe";
import {HttpErrorResponse} from "@angular/common/http";

export type TraceableApiRequest = { httpMethod: string; url: string, name: string, tag: string, summary: string | null };

const imports = [
  CommonModule,
  NgbNavModule,
  NgbCollapseModule,
  NgxJsonViewerModule,
  ClipboardModule,
  NgbNav,
  NgbNavOutlet,
  NgbNavItem,
  HighlightModule,
  CapitalizePipe,
];

@Component({
  selector: 'app-api-request',
  standalone: true,
  imports: imports,
  providers: [SqlFormatPipe],
  templateUrl: './api-request.component.html',
  styleUrl: './api-request.component.scss'
})
export class ApiRequestComponent implements OnInit, AfterViewInit {
  @Input({required: true}) apiRequest!: TraceableApiRequest;

  protected readonly JSON = JSON;
  protected executionTrace$?: Observable<ExecutionTrace>;
  protected isCollapsed = true;

  private codeOnly$ = new BehaviorSubject(true);

  constructor(
    private apiService: ApiService,
    private sqlFormatPipe: SqlFormatPipe,
    private hljs: HighlightJS,
  ) {
  }

  public ngOnInit(): void {
    this.executionTrace$ = this.codeOnly$
      .pipe(
        switchMap(codeOnly => this.apiService.getTrace(this.apiRequest.httpMethod, this.apiRequest.url, codeOnly)),
        catchError(error => this.handleHttpError(error)),
        map(trace => {
          for (const databaseTrace of trace.databaseTraces) {
            databaseTrace.sqlCommandsFormatted = databaseTrace.sqlCommands
              .map(s => this.sqlFormatPipe.transform(s, databaseTrace.databaseType))
              .join('\n\n');
          }
          return trace;
        }),
      );
  }

  public ngAfterViewInit(): void {
    // Workaround for https://github.com/MurhafSousli/ngx-highlightjs/issues/275
    setTimeout(() => {
      document.querySelectorAll('code').forEach((el: HTMLElement) => {
        this.hljs.highlightElement(el).subscribe();
      });
    }, 500)
  }

  protected execute(): void {
    this.codeOnly$.next(false);
    this.isCollapsed = false;
  }

  private handleHttpError(error: HttpErrorResponse): Observable<ExecutionTrace> {
    const errorResponse: ExecutionTrace = {
      sourcecode: 'ERROR', databaseTraces: [{
        databaseType: 'sql',
        sqlCommands: [],
        sqlCommandsFormatted: 'ERROR',
        result: error,
        resultJson: JSON.stringify(error)
      }],
    };

    return of(errorResponse);
  }
}
