const coll = document.querySelectorAll(".contentBox")
const coll_content = document.querySelectorAll(".content")
coll.forEach(btn => {
    btn.addEventListener('click', ()=>{

        const panel = btn.nextElementSibling;
        panel.classList.toggle("active");
        btn.classList.toggle("active");
    })
})