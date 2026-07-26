document.querySelectorAll('th.sortable').forEach((th) => {
    th.addEventListener('click', () => {
      const down = th.querySelector('.ri-arrow-down-line');
      const up = th.querySelector('.ri-arrow-up-line');

      // toggle between up and down icons
      down.classList.toggle('d-none');
      up.classList.toggle('d-none');

      // reset other ths
      document.querySelectorAll('th.sortable').forEach((el) => {
        if (el !== th) {
          el.querySelector('.ri-arrow-down-line').classList.remove('d-none');
          el.querySelector('.ri-arrow-up-line').classList.add('d-none');
        }
      });
    });
  });
  