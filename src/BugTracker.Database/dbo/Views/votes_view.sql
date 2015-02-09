
create view votes_view as
select bu_bug as vote_bug, sum(bu_vote) as vote_total
from bug_user
group by bu_bug
having sum(bu_vote) > 0