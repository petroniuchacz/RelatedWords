import {isUndefined} from 'lodash';

const isDefinedOrNull = item => {
  if (isUndefined(item)) {
    return null;
  } else {
    return item
  }
}

export default isDefinedOrNull;