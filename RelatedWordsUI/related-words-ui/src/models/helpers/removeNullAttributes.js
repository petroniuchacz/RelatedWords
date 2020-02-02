function removeNullAttributes(obj) {
  for (var propName in obj) { 
    if (obj[propName] === null) {
      delete obj[propName];
    }
  }
  
  return obj;
}

export default removeNullAttributes;