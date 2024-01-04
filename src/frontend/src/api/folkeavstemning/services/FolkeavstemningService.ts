/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { FolkeavstemningDto } from '../models/FolkeavstemningDto';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class FolkeavstemningService {

    /**
     * @returns FolkeavstemningDto 
     * @throws ApiError
     */
    public static folkeavstemningGetFolkeavstemning(): CancelablePromise<Array<FolkeavstemningDto>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/folkeavstemning',
        });
    }

}
