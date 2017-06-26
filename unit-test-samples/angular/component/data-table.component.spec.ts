import { async, ComponentFixture, TestBed } from '@angular/core/testing'
import { DataTableComponent } from './data-table.component'
import { NO_ERRORS_SCHEMA } from '@angular/core'
import { DataTableAccessor } from 'app/shared/data-table/data-table-accessor'
import { Subject } from 'rxjs/Subject'
import { ToastsManager } from 'ng2-toastr'
import { ModalConfirmDeleteComponent } from '../modal-confirm-delete/modal-confirm-delete.component'
import { MockModalDirective } from 'test-utils'
import { By } from '@angular/platform-browser'
import { range } from 'lodash'
import { inject } from '@angular/core/testing'
import { DataTableAccessorData, SortDirection } from 'types'
import { Observable } from 'rxjs/Observable'
import { fakeAsync } from '@angular/core/testing'
import { tick } from '@angular/core/testing'

class MockAccessor implements DataTableAccessor {
  data = new Subject()
  params = new Subject()
  dataSnapshot: any

  fetch(_params?) {
    return this.data
  }

  remove(_item) {
    return this.data
  }

  mockData(data: DataTableAccessorData<any>) {
    this.dataSnapshot = data
    this.data.next(this.dataSnapshot)
  }
}

const mockToastr = {
  success() {},
  error() {}
}

describe('DataTableComponent', () => {
  let component: DataTableComponent
  let fixture: ComponentFixture<DataTableComponent>
  let accessor: MockAccessor

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      schemas: [ NO_ERRORS_SCHEMA ],
      declarations: [
        DataTableComponent,
        ModalConfirmDeleteComponent,
        MockModalDirective,
      ],
      providers: [
        { provide: DataTableAccessor, useClass: MockAccessor },
        { provide: ToastsManager, useValue: mockToastr }
      ]
    })
    .compileComponents()
  }))

  beforeEach(() => {
    fixture = TestBed.createComponent(DataTableComponent)
    component = fixture.componentInstance
  })

  beforeEach(inject([DataTableAccessor], (_accessor) => {
    accessor = _accessor
  }))

  it('should be created', () => {
    /**
     * move detech changes here because I want to test some component setup
     * like input
     */
    fixture.detectChanges()
    expect(component).toBeTruthy()
    expect(component.usePageLink).toBe(true, 'default to use page link')
  })

  it('should render essential component', () => {
    fixture.detectChanges()
    const de = fixture.debugElement

    const quantityElm = de.query(By.css('data-table-quantity'))
    expect(quantityElm).toBeTruthy('should render quantity selection box')

    const footerElm = de.query(By.css('data-table-footer'))
    expect(footerElm).toBeTruthy('should render table footer')

    const searchBoxElm = de.query(By.css('input[type="search"]'))
    expect(searchBoxElm).toBeTruthy('should render search box')

    const modalConfirm = de.query(By.directive(ModalConfirmDeleteComponent))
    expect(modalConfirm).toBeTruthy('should render modal confirm')
  })

  it(`should update #list to reflect accessor data`, async(() => {
    fixture.detectChanges() // after this line component ngOnInit lifecycle should be finished
    const items = range(0, 3).map(idx => ({
      id: idx,
      name: `item ${idx}`,
    }))

    const paging = {
      page: 1,
      quantity: 3,
      sortBy: 'name',
      total: 100,
      sortDirection: SortDirection.ascending,
    }

    accessor.mockData({
      items,
      paging,
      filter: {},
    })

    fixture.whenStable().then(() => {
      expect(component.list).toBeTruthy('table list should exist')
      expect(component.list).toEqual(items)
      expect(component.paging).toEqual(paging)
    })
  }))

  it('should fetch data when #updateParams()', () => {
    spyOn(accessor, 'fetch').and.callThrough()
    fixture.detectChanges()
    component.updateParams({page: 1, quantity: 5})

    fixture.whenStable().then(() => {
      expect(accessor.fetch).toHaveBeenCalledWith({page: 1, quantity: 5})
    })
  })

  it('should calculate proper item index', () => {
    fixture.detectChanges()
    component.paging = {
      page: 2,
      quantity: 10
    }

    expect(component.index(3)).toBe(14, 'item #3 (zero based index) of page 2 should be #14')
  })

  it('should search for item when text entered to search box', () => {
    spyOn(accessor, 'fetch').and.callThrough()
    fixture.detectChanges()
    const de = fixture.debugElement
    const searchBoxElm = de.query(By.css('input[type="search"]')).nativeElement as HTMLInputElement
    searchBoxElm.value = 'test term'

    /**
     * unfortunately you cannot emit a new KeyboardEvent() here
     * due to the fact that KeyboardEvent.keyCode is readonly and unmutable
     */
    const keyupEv = new Event('keyup') as any
    keyupEv.keyCode = 13
    searchBoxElm.dispatchEvent(keyupEv)
    expect(accessor.fetch).toHaveBeenCalledWith({query: searchBoxElm.value})
  })

  it('should not call search when user haven\'t finished enter keyword', () => {
    spyOn(accessor, 'fetch').and.callThrough()
    fixture.detectChanges()
    const de = fixture.debugElement
    const searchBoxElm = de.query(By.css('input[type="search"]')).nativeElement as HTMLInputElement
    searchBoxElm.value = 'test term'
    const keyupEv = new Event('keyup') as any
    keyupEv.keyCode = 65
    searchBoxElm.dispatchEvent(keyupEv)
    expect(accessor.fetch).not.toHaveBeenCalled()
  })

  it('should call accessor.fetch without param when #reload()', () => {
    spyOn(accessor, 'fetch').and.callThrough()
    fixture.detectChanges()
    component.reload()
    expect(accessor.fetch).toHaveBeenCalledWith()
  })

  it('should open delete confirmation modal when calling #delete()', () => {
    const de = fixture.debugElement
    const modalConfirm = de.query(By.directive(ModalConfirmDeleteComponent))
    spyOn(modalConfirm.componentInstance, 'open').and.callFake(() => {})

    const item = { name: 'item 1', id: '1' }
    component.delete(item)
    expect(modalConfirm.componentInstance.open).toHaveBeenCalledWith(item)
  })

  it('should call remove after delete confirmed', () => {
    spyOn(accessor, 'remove').and.callThrough()
    const de = fixture.debugElement
    const modalConfirm = de.query(By.directive(ModalConfirmDeleteComponent))
    const item = { name: 'item 1', id: '1' }
    modalConfirm.triggerEventHandler('deleteConfirmed', item)

    expect(accessor.remove).toHaveBeenCalledWith(item)
  })

  const willRemove = item => {
    const de = fixture.debugElement
    const modalConfirm = de.query(By.directive(ModalConfirmDeleteComponent))
    modalConfirm.triggerEventHandler('deleteConfirmed', item)
  }

  it('should show toast of type success when remove successfully', fakeAsync(inject([ToastsManager], (toastr) => {
    fixture.detectChanges()
    spyOn(accessor, 'remove').and.returnValue(Observable.of(undefined))
    const toastSuccessSpy = spyOn(toastr, 'success')
    const item = { name: 'item 1', id: '1' }
    willRemove(item)
    fixture.detectChanges()
    tick()
    expect(toastSuccessSpy).toHaveBeenCalledWith(jasmine.any(String), 'success')
    expect(toastSuccessSpy.calls.mostRecent().args[0]).toMatch(/item 1 deleted successfully/, 'should show default message')
  })))

  it('should show toast of type error when remove failed', fakeAsync(inject([ToastsManager], (toastr) => {
    fixture.detectChanges()
    spyOn(accessor, 'remove').and.returnValue(Observable.throw({messages: ['item delete failed']}))
    const toastErrorSpy = spyOn(toastr, 'error')
    const item = { name: 'item 1', id: '1' }
    willRemove(item)
    fixture.detectChanges()
    tick()
    expect(toastErrorSpy).toHaveBeenCalledWith(jasmine.any(String), 'error')
    expect(toastErrorSpy.calls.mostRecent().args[0]).toMatch(/item delete failed/, 'should show received message')
  })))

  it('#clearSearch() should clear query params in api call', () => {
    fixture.detectChanges()
    const fetchSpy = spyOn(accessor, 'fetch')
    component.clearSearch()
    expect(fetchSpy).toHaveBeenCalled()
    expect(fetchSpy.calls.mostRecent().args[0].query).toBeUndefined()
  })

  it(`if DO NOT use page link: changePage should call 'DataTableAccessor.fetch()'`, () => {
    component.usePageLink = false
    const fetchSpy = spyOn(accessor, 'fetch')
    fixture.detectChanges()
    const de = fixture.debugElement
    const footerElm = de.query(By.css('data-table-footer'))
    footerElm.triggerEventHandler('pageChanged', 3)
    expect(fetchSpy).toHaveBeenCalledWith(jasmine.objectContaining({page: 3}))
  })

  it(`if DO use page link: changePage should not call 'DataTableAccessor.fetch()'`, () => {
    const fetchSpy = spyOn(accessor, 'fetch')
    fixture.detectChanges()
    const de = fixture.debugElement
    const footerElm = de.query(By.css('data-table-footer'))
    footerElm.triggerEventHandler('pageChanged', 3)
    expect(fetchSpy).not.toHaveBeenCalled()
  })
})
