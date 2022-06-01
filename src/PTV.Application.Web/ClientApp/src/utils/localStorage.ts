// Snippet from https://developer.mozilla.org/en-US/docs/Web/API/Web_Storage_API/Using_the_Web_Storage_API#testing_for_availability
export const localStorageAvailable = (): boolean | undefined => {
  let storage;
  try {
    storage = localStorage;
    const storageTest = '__storage_test__';
    storage.setItem(storageTest, storageTest);
    storage.removeItem(storageTest);
    return true;
  } catch (e) {
    return (
      e instanceof DOMException &&
      // everything except Firefox
      (e.code === 22 ||
        // Firefox
        e.code === 1014 ||
        // test name field too, because code might not be present
        // everything except Firefox
        e.name === 'QuotaExceededError' ||
        // Firefox
        e.name === 'NS_ERROR_DOM_QUOTA_REACHED') &&
      // acknowledge QuotaExceededError only if there's something already stored
      storage &&
      storage.length !== 0
    );
  }
};

export const getLocalStorageItem = (path: string): string | null => {
  try {
    const result = localStorage.getItem(path);
    return result;
  } catch (error) {
    console.error(error);
    return null;
  }
};

export const setLocalStorageItem = (path: string, data: string) => {
  try {
    localStorage.setItem(path, data);
  } catch (error) {
    console.error(error);
  }
};
