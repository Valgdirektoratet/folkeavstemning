/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { SignertStemmeDto } from '../models/SignertStemmeDto';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class ValgUrneService {

    /**
     * Avlegg stemme. Stemmen må være signert
     * @param folkeavstemningId 
     * @param requestBody 
     * @returns any 
     * @throws ApiError
     */
    public static valgUrneLeggInnStemme(
folkeavstemningId: string,
requestBody: SignertStemmeDto,
): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/folkeavstemning/{folkeavstemningId}',
            path: {
                'folkeavstemningId': folkeavstemningId,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }

}
