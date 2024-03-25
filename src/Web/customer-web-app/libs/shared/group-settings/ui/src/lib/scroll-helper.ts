export default function scrollToBLock(blockId: string) {
  const block = document.getElementById(blockId);
  window &&
    window.scroll({
      top: block?.offsetTop,
      behavior: 'smooth',
    });
}
