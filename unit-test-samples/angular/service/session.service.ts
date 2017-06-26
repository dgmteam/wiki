import {
  Injectable
} from '@angular/core'
import {
  storage
} from 'utils/local-storage'

@Injectable()
export class SessionService {

  private appDataStorageKey = 'Newsroom'
  private sessionData

  constructor() {
    this.init()
  }

  refreshSession(data) {
    this.sessionData = data
    this.sync()
  }

  get(key: string) {
    const sessionData = storage.get(this.appDataStorageKey) || {}
    const value = sessionData[key]
    return value
  }

  set(key: string, value: any) {
    this.sessionData[key] = value
    this.sync()
  }

  setValues(values: {
    [key: string]: any
  }) {
    for (let key in values) {
      if (values.hasOwnProperty(key)) {
        this.sessionData[key] = values[key]
      }
    }
    this.sync()
  }

  remove(keys: string[]) {
    keys.forEach(k => {
      delete this.sessionData[k]
    })
    this.sync()
  }

  sync() {
    storage.set(this.appDataStorageKey, this.sessionData)
  }

  private init() {
    this.sessionData = storage.get(this.appDataStorageKey) || {}
  }

}