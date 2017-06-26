import { MockBackend } from '@angular/http/testing'
import { RouterTestingModule } from '@angular/router/testing'
import { TestBed, inject } from '@angular/core/testing'
import { XHRBackend, HttpModule } from '@angular/http'

import { IAccount } from 'types'
import { JournalistApi } from './journalist-api.service'
import { mockResponse, apiServiceTestProviders } from 'testutils'
import { SessionService, ErrorHandlerService } from 'app/core'

describe('JournalistApi', () => {
  let service: JournalistApi
  let mockBackend: MockBackend

  const mockRes: IAccount = {
    id: 'id',
    createdDate: 'createdDate',
    updatedDate: 'updatedDate',
    firstName: 'firstName',
    lastName: 'lastName',
    email: 'example@mail.com',
    password: 'DigiMed123',
    status: true,
    phoneNumber: '0'
  }

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        apiServiceTestProviders,
        ErrorHandlerService,
        JournalistApi,
        SessionService,
      ],
      imports: [
        HttpModule,
        RouterTestingModule,
      ]
    })
  })

  beforeEach(inject([JournalistApi, XHRBackend], (_service, _mockBackend) => {
    service = _service
    mockBackend = _mockBackend
  }))

  it('should be created', () => {
    expect(service).toBeTruthy()
  })

  it('should get own info', () => {
    mockResponse(mockBackend, mockRes)

    service.getOwn().subscribe(res => {
      expect(res).toBeTruthy()
      expect(res.id).toBe('id')
      expect(res.createdDate).toBe('createdDate')
      expect(res.updatedDate).toBe('updatedDate')
      expect(res.firstName).toBe('firstName')
      expect(res.lastName).toBe('lastName')
      expect(res.email).toBe('example@mail.com')
      expect(res.password).toBe('DigiMed123')
      expect(res.status).toBe(true)
      expect(res.phoneNumber).toBe('0')
    })
  })

})