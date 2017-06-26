import { TestBed, inject } from '@angular/core/testing'

import { SessionService } from './session.service'
import { storage } from 'utils/local-storage'

describe('SessionService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        SessionService,
        { provide: storage, useValue: { set: {} } }
      ]
    })
  })

  it('should be created', inject([SessionService], (service: SessionService) => {
    expect(service).toBeTruthy()
  }))

  it('#refreshSession should be refresh session data', inject([SessionService, storage], (service: SessionService) => {
    spyOn(storage, 'set')
    service.refreshSession({ mockKey: '123'})
    expect(storage.set).toHaveBeenCalledWith('Newsroom', { mockKey: '123'})
  }))

  it('#get should be return value of input key in localstorage', inject([SessionService, storage], (service: SessionService) => {
    spyOn(storage, 'get').and.returnValue({mockKey: '123'})
    let mockValue = service.get('mockKey')
    expect(mockValue).toBe('123')
  }))

  it('#set should be set value of input key to lacalstorage', inject([SessionService, storage], (service: SessionService) => {
    spyOn(storage, 'set')
    service.set('mockKey', '456')
    expect(storage.set).toHaveBeenCalledWith('Newsroom', { mockKey: '456' })
  }))

  it('#setValues should be set values to localstorage', inject([SessionService, storage], (service: SessionService) => {
    spyOn(storage, 'set')
    service.setValues({['mockKey']: '123', ['mockey2']: '456'})
    expect(storage.set).toHaveBeenCalledWith('Newsroom', { mockKey: '123', mockey2: '456' })
  }))

  it('#remove should be remove values to localstorage', inject([SessionService, storage], (service: SessionService) => {
    spyOn(storage, 'set')
    service.remove(['mockKey'])
    expect(storage.set).toHaveBeenCalledWith('Newsroom', {})
  }))
})
