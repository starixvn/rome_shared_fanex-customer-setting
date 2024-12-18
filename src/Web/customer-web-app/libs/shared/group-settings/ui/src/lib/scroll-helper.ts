export function scrollToBLock(blockId: string) {
  const block = document.getElementById(blockId);
  window &&
    window.scroll({
      top: block?.offsetTop,
      behavior: 'smooth',
    });
}

export const scrollToTop = () => {
  window &&
    window.scroll({
      top: 0,
      left: 0,
      behavior: 'smooth',
    });
};

export const scrollPageToTop = (w: Window) => {
  const scrollInfo:ScrollToOptions = {
    top: 0,
    left: 0,
    behavior: 'smooth',
  };

  w && w.scroll(scrollInfo);
};
