import { Injectable, Injector } from '@angular/core'

import { BaseApi } from './base-api.service'
import { IJournalistInfo } from 'types'

@Injectable()
export class JournalistApi extends BaseApi {

  constructor(injector: Injector) {
    super(injector)
    this.baseUrl = this.appConfig.resourceApiUrl + '/api/journalist'
  }

  getOwn() {
    return this.get('/getOwn')
  }

  updateOwn(params: IJournalistInfo) {
    return this.put('/updateOwn', undefined, params)
  }

}