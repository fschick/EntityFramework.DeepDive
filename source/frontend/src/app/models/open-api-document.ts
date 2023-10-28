// Generated using https://csharptotypescript.azurewebsites.net/

import {HttpStatusCode} from "@angular/common/http";
import {Dictionary} from "./dictionary";

export interface OpenApiDocument {
  info: OpenApiInfo;
  paths: Dictionary<string, Dictionary<keyof typeof OpenApiHttpMethod, OpenApiOperation>>;
  components: OpenApiComponents;
}

export interface OpenApiInfo {
  title: string | null;
  version: string | null;
}

export enum OpenApiHttpMethod {
  Get,
  Post,
  Put,
  Delete
}

export interface OpenApiOperation {
  tags: string[];
  summary: string | null;
  operationId: string | null;
  parameters: OpenApiParameter[];
  responses: Dictionary<HttpStatusCode, OpenApiResponse>;
}

export interface OpenApiParameter {
  name: string;
  in: OpenApiBinding;
  description: string | null;
}

export enum OpenApiBinding {
  Query,
  Header,
  Path,
  Cookie
}

export interface OpenApiResponse {
  description: string | null;
  content: Dictionary<string, OpenApiContent>;
}

export interface OpenApiContent {
  schema: OpenApiSchema;
}

export interface OpenApiComponents {
  schemas: Dictionary<string, OpenApiSchema>;
}

export interface OpenApiSchema {
  type: string | null;
  $ref: string | null;
}
