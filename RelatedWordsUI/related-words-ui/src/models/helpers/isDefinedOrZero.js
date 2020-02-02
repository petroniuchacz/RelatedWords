import {isUndefined} from 'lodash';

const isDefinedOrZero = item => {
  if (isUndefined(item)) {
    return 0;
  } else {
    return item
  }
}

export default isDefinedOrZero;