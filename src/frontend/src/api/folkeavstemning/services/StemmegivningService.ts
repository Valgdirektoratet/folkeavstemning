/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class StemmegivningService {

    /**
     * Setter kryss i manntall
     * @param folkeavstemningId FolkeavstemningsId der det skal avlegges stemme
     * @param requestBody BASE64 encoded bytes - stemmepakke
     * @returns string 
     * @throws ApiError
     */
    public static stemmegivningAvleggStemme(
folkeavstemningId: string,
requestBody: string,
): CancelablePromise<string> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/stemmegivning/{folkeavstemningId}',
            path: {
                'folkeavstemningId': folkeavstemningId,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }

}
