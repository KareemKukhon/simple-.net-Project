using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Dtos.Stock;
using api.interfaces;
using api.Mappers;
using api.models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route ("api/comment")]
    [ApiController]
    public class CommentController: ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var comments = await _commentRepo.GetAllAsync();
            var commentDto = comments.Select(s=>s.ToCommentDto());
            if(comments == null){
                return NotFound();
            }
            return Ok(commentDto);
        }

        [HttpGet]
        [Route ("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var comment = await _commentRepo.GetByIdAsync(id);
            if(comment == null){
                return NotFound();
            }
            return Ok(comment.ToCommentDto());
        }
        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentDto commentDto){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            bool isExist = await _stockRepo.StockExists(stockId);
            if(!isExist){
                return BadRequest("Stock does not exist");

            }
            var commentModel = commentDto.ToCommentFromCreate(stockId);
            await _commentRepo.CreateAsync(commentModel);
                
            return CreatedAtAction(nameof(GetById), new {id = commentModel.Id}, commentModel.ToCommentDto());
            
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCommnet([FromRoute]int id, [FromBody] UpdateCommentDto updateComment){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var comment = await _commentRepo.UpdateAsync(updateComment.ToCommentFromUpdate(), id);
            if(comment == null){
                return NotFound();
            }
            return Ok(comment.ToCommentDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteComment([FromRoute]int id){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var commentModel = await _commentRepo.DeleteAsync(id);
            if(commentModel == null){
                return NotFound();
            }
            return Ok();
        }
    }
}