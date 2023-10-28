import {ApplicationConfig} from '@angular/core';
import {provideRouter} from '@angular/router';
import {routes} from './app.routes';
import {provideHttpClient} from "@angular/common/http";
import {HIGHLIGHT_OPTIONS} from "ngx-highlightjs";

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    {
      provide: HIGHLIGHT_OPTIONS,
      useValue: {
        // Workaround for https://github.com/MurhafSousli/ngx-highlightjs/issues/275
        fullLibraryLoader: () => import('highlight.js'),
        // coreLibraryLoader: () => import('highlight.js/lib/core'),
        // languages: {
        //         //   cs: () => import('highlight.js/lib/languages/csharp'),
        //         //   sql: () => import('highlight.js/lib/languages/sql'),
        //         // }
      }
    }
  ]
};
