/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Stemmerett } from '../models/Stemmerett';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class StemmerettService {

    /**
     * @returns string 
     * @throws ApiError
     */
    public static stemmerettGetUrlToStemmemottak(): CancelablePromise<string> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/stemmerett/stemmemottak-url',
        });
    }

    /**
     * Sjekker hvilke folkeavstemninger brukeren har stemmerett i
     * @returns Stemmerett Stemmerett for brukeren i angitt folkeavstemning
     * @throws ApiError
     */
    public static stemmerettGetStemmerett(): CancelablePromise<Record<string, Stemmerett>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/stemmerett/stemmerett',
            errors: {
                401: `Brukeren er ikke autorisert`,
                404: `Ugyldig folkeavstemning`,
            },
        });
    }

}
