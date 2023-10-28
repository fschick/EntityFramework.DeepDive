import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ApiService} from "../../services/api.service";
import {map, Observable} from "rxjs";
import {OpenApiHttpMethod, OpenApiOperation} from "../../models/open-api-document";
import {HttpStatusCode} from "@angular/common/http";
import {ApiRequestComponent, TraceableApiRequest} from "../../components/api-request/api-request.component";
import {Dictionary} from "../../models/dictionary";

type TraceableApiGroup = { name: string, requests: TraceableApiRequest[] };

@Component({
  selector: 'app-blog',
  standalone: true,
  imports: [CommonModule, ApiRequestComponent],
  templateUrl: './blog.component.html',
  styleUrl: './blog.component.scss'
})
export class BlogComponent {
  public traceableApiGroups$: Observable<TraceableApiGroup[]>;

  constructor(
    private apiService: ApiService
  ) {
    this.traceableApiGroups$ = this.getTraceableApiRequests();
  }

  private getTraceableApiRequests(): Observable<TraceableApiGroup[]> {
    return this.apiService
      .getOpenApiSpec()
      .pipe(
        map(spec => {
            const operations = Object
              .entries(spec.paths)
              .flatMap(([url, operations]) => this.getOperations(url, operations));

            const groups = groupBy(operations, operation => operation.tag);
            return Object
              .entries(groups)
              .map(([group, requests]) => ({name: group, requests}));
          }
        ),
      )
  }

  private getOperations(url: string, operations: Dictionary<keyof typeof OpenApiHttpMethod, OpenApiOperation>): TraceableApiRequest[] {
    return this.getOperationsDetails(operations)
      .map(operationDetails => ({url, name: url.split('/').at(-1)!, ...operationDetails}));
  }

  private getOperationsDetails(operations: Dictionary<keyof typeof OpenApiHttpMethod, OpenApiOperation>): Omit<TraceableApiRequest, 'url' | 'name'>[] {
    return Object
      .entries(operations)
      .filter(([, operation]) => this.hasSingleParameterCodeOnly(operation))
      .filter(([, operation]) => this.hasExecutionTraceResult(operation))
      .map(([httpMethod, operation]) =>
        operation.tags.map(tag => ({
          httpMethod: httpMethod,
          tag: tag,
          summary: operation.summary
        })))
      .flatMap(operations => operations);
  }

  private hasSingleParameterCodeOnly(operation: OpenApiOperation): boolean {
    return operation.parameters && operation.parameters[0]?.name == "codeOnly";
  }

  private hasExecutionTraceResult(operation: OpenApiOperation): boolean {
    const content = operation.responses[HttpStatusCode.Ok]?.content;
    if (content === undefined)
      return false;
    const returnType = content['application/json']?.schema.$ref;
    return returnType != undefined && returnType.indexOf('ExecutionTrace') >= 0;
  }

}

const groupBy = <T>(array: T[], predicate: (value: T, index: number, array: T[]) => string) =>
  array.reduce((acc, value, index, array) => {
    (acc[predicate(value, index, array)] ||= []).push(value);
    return acc;
  }, {} as { [key: string]: T[] });
