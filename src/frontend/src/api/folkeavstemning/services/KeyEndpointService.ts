/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { VoteKeys } from '../models/VoteKeys';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class KeyEndpointService {

    /**
     * Henter public krypteringssertifikat for valget
     * @param folkeavstemningId 
     * @returns VoteKeys 
     * @throws ApiError
     */
    public static keyEndpointGetKeys(
folkeavstemningId: string,
): CancelablePromise<VoteKeys> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/keys/{folkeavstemningId}/encryption/public',
            path: {
                'folkeavstemningId': folkeavstemningId,
            },
            errors: {
                401: `Brukeren er ikke autorisert`,
                403: `Har ikke stemmerett`,
                404: `Ugyldig folkeavstemning`,
                500: `Manglende nøkkel-konfigurasjon`,
            },
        });
    }

}
