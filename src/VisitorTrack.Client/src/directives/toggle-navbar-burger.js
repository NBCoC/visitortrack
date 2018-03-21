export default {
  bind: el => {
    const onclick = event => {
      event.stopPropagation();

      const target = document.getElementById(el.dataset.target);

      el.classList.toggle('is-active');

      if (!target) return;

      target.classList.toggle('is-active');
    };

    el.addEventListener('click', onclick);
  }
};
