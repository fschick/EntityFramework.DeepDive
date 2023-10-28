import {Injectable, isDevMode} from '@angular/core';
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {OpenApiDocument} from "../models/open-api-document";
import {ExecutionTrace} from "../models/execution-trace";

@Injectable({
    providedIn: 'root'
})
export class ApiService {

    private static backendOrigin = isDevMode()
        ? `${location.protocol}//${location.hostname}:5050`
        : `${document.baseURI.replace(/\/$/, '')}`;

    public static backendApi = `${ApiService.backendOrigin}/api`;
    private static apiVersion = 'v1';

    constructor(
        private httpClient: HttpClient
    ) {
    }

    public getTrace(httpMethod: string, resource: string, codeOnly: boolean): Observable<ExecutionTrace> {
        const url = `${ApiService.backendOrigin}${resource}?codeOnly=${codeOnly}`;
        return this.httpClient.request<ExecutionTrace>(httpMethod, url);
    }

    public getOpenApiSpec(): Observable<OpenApiDocument> {
        return this.httpClient.get<OpenApiDocument>(`${ApiService.backendApi}/${ApiService.apiVersion}/openapi.json`);
    }

    public getProductName(): Observable<string> {
        return this.httpClient.get(`${ApiService.backendApi}/Information/GetProductName`, {responseType: 'text'});
    }
}
